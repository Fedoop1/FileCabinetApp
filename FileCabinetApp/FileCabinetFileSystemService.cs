using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Interfaces;

#pragma warning disable SA1305 // Field names should not use Hungarian notation

namespace FileCabinetApp
{
    /// <summary>
    /// Organizes work through a <see cref="FileStream"/> in a byte format. Is an alternative of <see cref="FileCabinetMemoryService"/>.
    /// </summary>
    public sealed class FileCabinetFilesystemService : IFileCabinetService, IRecordValidator, IDisposable
    {
        private const int MaxRecordLength = 255 + sizeof(short) + sizeof(decimal) + sizeof(char) + sizeof(bool);
        private const int MaxStringLength = 120;

        private readonly IRecordValidator validator;
        private readonly string fileName;
        private readonly FileStream fileStream;

        private readonly Dictionary<DateTime, List<int>> dateTimeOffsetDictionary = new ();
        private readonly Dictionary<int, int> idOffsetDictionary = new ();
        private readonly Dictionary<string, List<int>> firstNameOffsetDictionary = new (comparer: StringComparer.CurrentCultureIgnoreCase);
        private readonly Dictionary<string, List<int>> lastNameOffsetDictionary = new (comparer: StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class and set the file stream instance.
        /// </summary>
        /// <param name="fileName">Storage file name.</param>
        /// <param name="validator">Record validator.</param>
        public FileCabinetFilesystemService(string fileName, IRecordValidator validator)
        {
            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName), "File name can't be null");
            this.validator = validator ??
                                      throw new ArgumentNullException(nameof(validator), "Validator can't be null");
            this.fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            this.AnalyzeDatabase();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <returns></returns>
        ~FileCabinetFilesystemService() => this.Dispose(false);

        private enum RecordState : byte
        {
            Alive = 0,
            Deleted,
        }

        /// <inheritdoc/>
        public void AddRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            if (this.TryFindRecordById(record.Id).state == RecordState.Alive)
            {
                throw new ArgumentException("Record with this Id already exists");
            }

            byte[] recordByteArray = RecordToByteConverter(record);
            this.fileStream.Position = this.fileStream.Length;
            this.AddToIndexTable(record, (int)this.fileStream.Position);
            this.fileStream.Write(recordByteArray);
        }

        /// <inheritdoc/>
        public void EditRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            byte[] recordByteArray = RecordToByteConverter(record);

            this.fileStream.Position = this.TryFindRecordById(record.Id).position;
            this.fileStream.Write(recordByteArray);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth)
        {
            if (!DateTime.TryParse(dateOfBirth, out DateTime birthDate))
            {
                Console.WriteLine("Date of birth is incorrect!");
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.dateTimeOffsetDictionary[birthDate] is null)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.FindByDayOfBirthEnumerable(birthDate);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                Console.WriteLine("First name is incorrect!");
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.firstNameOffsetDictionary[firstName] is null)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.FindByFirstNameEnumerable(firstName);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("First name is incorrect!");
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.lastNameOffsetDictionary[lastName] is null)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.FindByLastNameEnumerable(lastName);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords() => this.GetRecords(new RecordQuery(_ => true, string.Empty)); // HashCode omitted cause FileStream service doesn't use caching mechanism.

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords(IRecordQuery query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query), "Records query can't be null");
            }

            return this.GetRecordsIterator(query);
        }

        /// <inheritdoc/>
        public (int AliveRecords, int DeletedRecords) GetStat()
        {
            int aliveCount = default;
            int deletedCount = default;

            this.fileStream.Position = default;
            using BinaryReader binaryReader = new (this.fileStream, Encoding.Default, true);
            while (this.fileStream.Position < this.fileStream.Length)
            {
                this.fileStream.Position += MaxRecordLength - sizeof(bool);

                if (binaryReader.ReadBoolean())
                {
                    deletedCount++;
                }
                else
                {
                    aliveCount++;
                }
            }

            return (aliveCount, deletedCount);
        }

        /// <inheritdoc/>
        public RecordSnapshot MakeSnapshot() => new (this.GetRecords());

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record) => this.validator.ValidateRecord(record);

        /// <inheritdoc/>
        public int Restore(RecordSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), "Restore snapshot is null");
            }

            int affectedRecordsCount = default;
            (FileCabinetRecord actualRecord, RecordState state, int position) recordData;
            foreach (var restoreRecord in snapshot.Records)
            {
                var recordByteArray = RecordToByteConverter(restoreRecord);
                if ((recordData = this.TryFindRecordById(restoreRecord.Id)).actualRecord != null)
                {
                    this.fileStream.Position = recordData.position;
                    this.fileStream.Write(recordByteArray);
                }
                else
                {
                    this.fileStream.Position = this.fileStream.Length;
                    this.fileStream.Write(recordByteArray);
                }

                affectedRecordsCount++;
            }

            return affectedRecordsCount;
        }

        /// <inheritdoc/>
        public void DeleteRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            var recordPosition = this.TryFindRecordById(record.Id).position;

            this.RemoveFromIndexTable(record, recordPosition);
            this.fileStream.Position = recordPosition;
            this.fileStream.Position += MaxRecordLength - sizeof(bool);
            this.fileStream.WriteByte((byte)RecordState.Deleted);
        }

        /// <inheritdoc/>
        public string Purge()
        {
            int purgedCount = default;
            byte[] aliveRecordBuffer = new byte[MaxRecordLength];
            byte[] deletedRecordBuffer = new byte[MaxRecordLength];
            var (aliveRecords, deletedRecords) = this.GetStat();
            (int Index, int Position) deletedRecordData;
            (int Index, int Position) aliveRecordData;

            while ((deletedRecordData = this.FindDeletedRecord()).Index != -1)
            {
                if ((aliveRecordData = this.FindAliveRecord()).Position < deletedRecordData.Position)
                {
                    this.fileStream.SetLength(this.fileStream.Length - MaxRecordLength);
                    purgedCount++;
                    continue;
                }

                this.fileStream.Position = deletedRecordData.Position;
                this.fileStream.Read(deletedRecordBuffer);
                this.fileStream.Position = aliveRecordData.Position;
                this.fileStream.Read(aliveRecordBuffer);
                this.fileStream.Position = deletedRecordData.Position;
                this.fileStream.Write(aliveRecordBuffer);
                this.fileStream.Position = aliveRecordData.Position;
                this.fileStream.Write(new byte[MaxRecordLength]);

                this.UpdateIndexTableLinks(FromByteToRecord(aliveRecordBuffer, out _), aliveRecordData.Position, deletedRecordData.Position);
                this.fileStream.SetLength(this.fileStream.Length - MaxRecordLength);
                purgedCount++;
            }

            return $"Data file processing is completed: {purgedCount} of {deletedRecords + aliveRecords} records were purged.";
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Convert <see cref="FileCabinetRecord"/> to byte representation.
        /// </summary>
        /// <param name="record">Record for conversion.</param>
        /// <returns>The byte array with byte representation of record.</returns>
        private static byte[] RecordToByteConverter(FileCabinetRecord record)
        {
            byte[] recordByteArray = new byte[MaxRecordLength];
            using var memoryStream = new MemoryStream(recordByteArray);
            using var binaryWriter = new BinaryWriter(memoryStream);

            var byteName = StringToBytes(record.FirstName);
            var byteLastName = StringToBytes(record.LastName);

            binaryWriter.Write(record.Id);
            binaryWriter.Write(byteName);
            binaryWriter.Write(byteLastName);
            binaryWriter.Write(record.DateOfBirth.Day);
            binaryWriter.Write(record.DateOfBirth.Month);
            binaryWriter.Write(record.DateOfBirth.Year);
            binaryWriter.Write(record.Height);
            binaryWriter.Write(record.Money);
            binaryWriter.Write(record.Gender);
            binaryWriter.Write(false);

            return recordByteArray;
        }

        private static FileCabinetRecord FromByteToRecord(byte[] bytes, out RecordState state)
        {
            using MemoryStream memStream = new (bytes);
            using BinaryReader binaryReader = new (memStream);

            var result = new FileCabinetRecord()
            {
                Id = binaryReader.ReadInt32(),
                FirstName = BytesToString(binaryReader.ReadBytes(MaxStringLength)),
                LastName = BytesToString(binaryReader.ReadBytes(MaxStringLength)),
                DateOfBirth = new DateTime(day: binaryReader.ReadInt32(), month: binaryReader.ReadInt32(), year: binaryReader.ReadInt32()),
                Height = binaryReader.ReadInt16(),
                Money = binaryReader.ReadDecimal(),
                Gender = binaryReader.ReadChar(),
            };

            bool isDeleted = binaryReader.ReadBoolean();
            state = isDeleted ? RecordState.Deleted : RecordState.Alive;
            return result;
        }

        /// <summary>
        /// Convert string to bytes representation.
        /// </summary>
        /// <param name="text">String to convert.</param>
        /// <returns>Byte array of string.</returns>
        private static byte[] StringToBytes(string text)
        {
            var result = new byte[MaxStringLength];
            var asciiText = Encoding.ASCII.GetBytes(text);
            int textLength = text.Length;

            if (textLength > MaxStringLength)
            {
                textLength = MaxStringLength;
            }

            Array.Copy(asciiText, result, textLength);
            return result;
        }

        /// <summary>
        /// Convert byte array to to string representation.
        /// </summary>
        /// <param name="byteName">Source array.</param>
        /// <returns>String result.</returns>
        private static string BytesToString(byte[] byteName)
        {
            var result = Encoding.ASCII.GetString(byteName);
            return result.TrimEnd('\0');
        }

        private IEnumerable<FileCabinetRecord> FindByLastNameEnumerable(string lastName)
        {
            var recordBuffer = new byte[MaxRecordLength];
            foreach (var position in this.lastNameOffsetDictionary[lastName])
            {
                this.fileStream.Position = position;
                this.fileStream.Read(recordBuffer);
                yield return FromByteToRecord(recordBuffer, out _);
            }
        }

        private IEnumerable<FileCabinetRecord> GetRecordsIterator(IRecordQuery query)
        {
            var recordBuffer = new byte[MaxRecordLength];

            this.fileStream.Position = default;
            while (this.fileStream.Position != this.fileStream.Length)
            {
                this.fileStream.Read(recordBuffer);
                var record = FromByteToRecord(recordBuffer, out var state);

                if (state == RecordState.Alive && query.Predicate(record))
                {
                    yield return record;
                }
            }
        }

        private (int index, int position) FindDeletedRecord()
        {
            this.fileStream.Position = default;
            using var binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true);
            while (this.fileStream.Position < this.fileStream.Length)
            {
                this.fileStream.Position += MaxRecordLength - sizeof(bool);
                if (binaryReader.ReadBoolean())
                {
                    this.fileStream.Position -= MaxRecordLength;
                    return (binaryReader.ReadInt32(), (int)(this.fileStream.Position - sizeof(int)));
                }
            }

            return (-1, -1);
        }

        private (int index, int position) FindAliveRecord()
        {
            using var binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true);
            this.fileStream.Position = this.fileStream.Length;
            while (this.fileStream.Position > 0)
            {
                this.fileStream.Position -= sizeof(bool);
                if (binaryReader.ReadBoolean() == false)
                {
                    this.fileStream.Position -= MaxRecordLength;
                    return (binaryReader.ReadInt32(), (int)(this.fileStream.Position - sizeof(int)));
                }

                this.fileStream.Position -= MaxRecordLength;
            }

            return (-1, -1);
        }

        private (FileCabinetRecord actualRecord, RecordState state, int position) TryFindRecordById(int id)
        {
            byte[] recordBuffer = new byte[MaxRecordLength];
            using BinaryReader reader = new (this.fileStream, Encoding.Default, true);
            FileCabinetRecord record;

            if (this.idOffsetDictionary.ContainsKey(id))
            {
                this.fileStream.Position = this.idOffsetDictionary[id];
                this.fileStream.Read(recordBuffer);
                record = FromByteToRecord(recordBuffer, out var state);
                return (record, state, this.idOffsetDictionary[id]);
            }

            this.fileStream.Position = default;
            while (this.fileStream.Position < this.fileStream.Length)
            {
                if (reader.ReadInt32() == id)
                {
                    this.fileStream.Position -= sizeof(int);
                    this.fileStream.Read(recordBuffer);
                    record = FromByteToRecord(recordBuffer, out var state);
                    return (record, state, (int)(this.fileStream.Position - MaxRecordLength));
                }

                this.fileStream.Position += MaxRecordLength - sizeof(int);
            }

            return (null, RecordState.Deleted, -1);
        }

        private void AnalyzeDatabase()
        {
            var recordBuffer = new byte[MaxRecordLength];

            this.fileStream.Position = default;
            while (this.fileStream.Position < this.fileStream.Length)
            {
                this.fileStream.Read(recordBuffer);
                var record = FromByteToRecord(recordBuffer, out RecordState state);
                int position = (int)(this.fileStream.Position - MaxRecordLength);

                if (state == RecordState.Alive)
                {
                    if (this.dateTimeOffsetDictionary.ContainsKey(record.DateOfBirth))
                    {
                        this.dateTimeOffsetDictionary[record.DateOfBirth].Add(position);
                    }
                    else
                    {
                        this.dateTimeOffsetDictionary[record.DateOfBirth] = new List<int> { position };
                    }

                    if (this.firstNameOffsetDictionary.ContainsKey(record.FirstName))
                    {
                        this.firstNameOffsetDictionary[record.FirstName].Add(position);
                    }
                    else
                    {
                        this.firstNameOffsetDictionary[record.FirstName] = new List<int> { position };
                    }

                    if (this.lastNameOffsetDictionary.ContainsKey(record.LastName))
                    {
                        this.lastNameOffsetDictionary[record.LastName].Add(position);
                    }
                    else
                    {
                        this.lastNameOffsetDictionary[record.LastName] = new List<int> { position };
                    }

                    this.idOffsetDictionary[record.Id] = position;
                }
            }
        }

        private IEnumerable<FileCabinetRecord> FindByDayOfBirthEnumerable(DateTime dateOfBirth)
        {
            var recordBuffer = new byte[MaxRecordLength];
            foreach (var position in this.dateTimeOffsetDictionary[dateOfBirth])
            {
                this.fileStream.Position = position;
                this.fileStream.Read(recordBuffer);
                yield return FromByteToRecord(recordBuffer, out _);
            }
        }

        private void RemoveFromIndexTable(FileCabinetRecord record, int position)
        {
            this.dateTimeOffsetDictionary[record.DateOfBirth].RemoveAll(x => x == position);
            this.firstNameOffsetDictionary[record.FirstName].RemoveAll(x => x == position);
            this.lastNameOffsetDictionary[record.LastName].RemoveAll(x => x == position);
            this.idOffsetDictionary.Remove(record.Id);
        }

        private void AddToIndexTable(FileCabinetRecord record, int position)
        {
            if (this.dateTimeOffsetDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateTimeOffsetDictionary[record.DateOfBirth].Add(position);
            }
            else
            {
                this.dateTimeOffsetDictionary[record.DateOfBirth] = new List<int>() { position };
            }

            if (this.firstNameOffsetDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameOffsetDictionary[record.FirstName].Add(position);
            }
            else
            {
                this.firstNameOffsetDictionary[record.FirstName] = new List<int>() { position };
            }

            if (this.lastNameOffsetDictionary.ContainsKey(record.LastName))
            {
                this.lastNameOffsetDictionary[record.LastName].Add(position);
            }
            else
            {
                this.lastNameOffsetDictionary[record.LastName] = new List<int>() { position };
            }

            this.idOffsetDictionary[record.Id] = position;
        }

        private void UpdateIndexTableLinks(FileCabinetRecord record, int oldPosition, int newPosition)
        {
            this.RemoveFromIndexTable(record, oldPosition);
            this.AddToIndexTable(record, newPosition);
        }

        private void ValidateInputRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null");
            }

            if (!this.ValidateRecord(record))
            {
                throw new ArgumentException("Record doesn't satisfy validation rules");
            }
        }

        private IEnumerable<FileCabinetRecord> FindByFirstNameEnumerable(string firstName)
        {
            var recordBuffer = new byte[MaxRecordLength];
            foreach (var position in this.firstNameOffsetDictionary[firstName])
            {
                this.fileStream.Position = position;
                this.fileStream.Read(recordBuffer);
                yield return FromByteToRecord(recordBuffer, out _);
            }
        }

        private void Dispose(bool disposing) => this.fileStream?.Dispose();
    }
}
