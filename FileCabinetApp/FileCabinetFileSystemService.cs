using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Organizes work through a <see cref="FileStream"/> in a byte format. Is an alternative of <see cref="FileCabinetMemoryService"/>.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IRecordValidator
    {
        private const int MaxRecordLength = 255 + sizeof(short) + sizeof(decimal) + sizeof(char) + sizeof(bool);
        private const int MaxStringLength = 120;

        private readonly IValidationSettings validationSettings;
        private readonly string fileName;

        private readonly Dictionary<DateTime, List<int>> dateTimeOffsetDictionary = new ();
        private readonly Dictionary<int, int> idOffsetDictionary = new ();
        private readonly Dictionary<string, List<int>> firstNameOffsetDictionary = new (comparer: StringComparer.CurrentCultureIgnoreCase);
        private readonly Dictionary<string, List<int>> lastNameOffsetDictionary = new (comparer: StringComparer.CurrentCultureIgnoreCase);

        private enum RecordState : byte
        {
            Alive = 0,
            Deleted,
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class and set the file stream instance.
        /// </summary>
        /// <param name="fileName">Storage file name.</param>
        /// <param name="settings">Validation settings.</param>
        public FileCabinetFilesystemService(string fileName, IValidationSettings settings)
        {
            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName), "File name can't be null");
            this.validationSettings = settings ??
                                      throw new ArgumentNullException(nameof(settings), "Validation settings can't be null");

            this.AnalyseDatabase();
        }

        /// <inheritdoc/>
        public int CreateRecord()
        {
            var recordData = new FileCabinetRecordData(this.validationSettings);
            recordData.InputData();
            this.ValidateParameters(recordData);

            var record = new FileCabinetRecord
            {
                Id = this.FindNextIndex(),
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Height = recordData.Height,
                Money = recordData.Money,
                Gender = recordData.Gender,
            };

            byte[] recordByteArray = RecordToByteConverter(record);
            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Write);
            fileStream.Position = fileStream.Length > 0 ? fileStream.Length : 0;
            this.AddToIndexTable(record, (int)fileStream.Position);
            fileStream.Write(recordByteArray);

            Console.WriteLine($"Record successfully created. Record id #{record.Id}");
            return record.Id;
        }

        /// <inheritdoc/>
        public IRecordValidator CreateValidator()
        {
            return ValidatorBuilder.CreateValidator(this.validationSettings);
        }

        /// <inheritdoc/>
        public void EditRecord(int id)
        {
            var (record, recordState, position) = this.TryFindRecordById(id);

            if (record is null || recordState == RecordState.Deleted)
            {
                Console.WriteLine("Record doesn't exist.");
                return;
            }

            var recordData = new FileCabinetRecordData(this.validationSettings);
            recordData.InputData();
            this.ValidateParameters(recordData);

            var newRecord = new FileCabinetRecord
            {
                Id = id,
                FirstName = recordData?.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Height = recordData.Height,
                Money = recordData.Money,
                Gender = recordData.Gender,
            };

            byte[] recordByteArray = RecordToByteConverter(newRecord);

            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Write);
            fileStream.Position = position;
            fileStream.Write(recordByteArray);

            Console.WriteLine($"Record {id} successful update.");
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByDayOfBirth(string dateOfBirth)
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

            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            var recordBuffer = new byte[MaxRecordLength];
            var result = new FileCabinetRecord[this.dateTimeOffsetDictionary[birthDate].Count];

            int index = 0;
            foreach (var position in this.dateTimeOffsetDictionary[birthDate])
            {
                fileStream.Position = position;
                fileStream.Read(recordBuffer);
                result[index++] = FromByteToRecord(recordBuffer, out _);
            }

            return result;
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByFirstName(string firstName)
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

            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            var recordBuffer = new byte[MaxRecordLength];
            var result = new FileCabinetRecord[this.firstNameOffsetDictionary[firstName].Count];

            int index = 0;
            foreach (var position in this.firstNameOffsetDictionary[firstName])
            {
                fileStream.Position = position;
                fileStream.Read(recordBuffer);
                result[index++] = FromByteToRecord(recordBuffer, out _);
            }

            return result;
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByLastName(string lastName)
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

            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            var recordBuffer = new byte[MaxRecordLength];
            var result = new FileCabinetRecord[this.lastNameOffsetDictionary[lastName].Count];

            int index = 0;
            foreach (var position in this.lastNameOffsetDictionary[lastName])
            {
                fileStream.Position = position;
                fileStream.Read(recordBuffer);
                result[index++] = FromByteToRecord(recordBuffer, out _);
            }

            return result;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using var fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            var recordArray = new FileCabinetRecord[(int)Math.Ceiling((double)fileStream.Length / MaxRecordLength)];
            var recordBuffer = new byte[MaxRecordLength];
            int index = default;

            while (fileStream.Position != fileStream.Length)
            {
                fileStream.Read(recordBuffer);
                var record = FromByteToRecord(recordBuffer, out RecordState state);

                if (state == RecordState.Alive)
                {
                    recordArray[index++] = record;
                }
            }

            return Array.AsReadOnly(recordArray);
        }

        /// <inheritdoc/>
        public (int RecordsCount, int DeletedRecords) GetStat()
        {
            int recordsCount = default;
            int deletedRecords = default;
            byte[] recordBuffer = new byte[MaxRecordLength];

            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);

            while (fileStream.Position < fileStream.Length)
            {
                fileStream.Read(recordBuffer);
                recordsCount++;
                if (recordBuffer[^1] == (byte)RecordState.Deleted)
                {
                    deletedRecords++;
                }
            }

            return (recordsCount, deletedRecords);
        }

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetServiceSnapshot"/> which contain all information about exists records.
        /// </summary>
        /// <returns>Instance of <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var recordsArray = this.GetRecords().ToArray();
            return new FileCabinetServiceSnapshot(recordsArray);
        }

        /// <inheritdoc/> 
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            this.CreateValidator().ValidateParameters(recordData);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot restoreSnapshot)
        {
            if (restoreSnapshot is null)
            {
                throw new ArgumentNullException(nameof(restoreSnapshot), "Restore snapshot is null");
            }

            var restoreRecordsList = restoreSnapshot.Records;
            var recordList = this.GetRecords();
            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            foreach (var restoreRecord in restoreRecordsList)
            {
                byte[] recordByteArray = RecordToByteConverter(restoreRecord);

                if (recordList.Any(record => record.Id == restoreRecord.Id))
                {
                    fileStream.Position = this.TryFindRecordById(restoreRecord.Id).position;
                    fileStream.Write(recordByteArray);
                }
                else
                {
                    fileStream.Write(recordByteArray);
                }
            }
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int index)
        {
            byte[] recordBuffer = new byte[MaxRecordLength];
            var (record, state, position) = this.TryFindRecordById(index);

            if (record is null || state == RecordState.Deleted)
            {
                return false;
            }

            this.RemoveFromIndexTable(record, position);

            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Position = position;
            fileStream.Read(recordBuffer);
            recordBuffer[MaxRecordLength - 1] = (byte)RecordState.Deleted;
            fileStream.Position = position;
            fileStream.Write(recordBuffer);

            return true;
        }

        /// <inheritdoc/>
        public string Purge()
        {
            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);

            int purgedCount = default;
            byte[] aliveRecordBuffer = new byte[MaxRecordLength];
            byte[] deletedRecordBuffer = new byte[MaxRecordLength];

            (int Index, int Position) deletedRecordData;
            (int Index, int Position) aliveRecordData;
            while ((deletedRecordData = this.FindDeletedRecord()).Index != -1)
            {
                if ((aliveRecordData = this.FindAliveRecord()).Position < deletedRecordData.Position)
                {
                    break;
                }

                fileStream.Position = deletedRecordData.Position;
                fileStream.Read(deletedRecordBuffer);
                fileStream.Position = aliveRecordData.Position;
                fileStream.Read(aliveRecordBuffer);
                fileStream.Position = deletedRecordData.Position;
                fileStream.Write(aliveRecordBuffer);

                this.idOffsetDictionary[aliveRecordData.Index] = deletedRecordData.Position;
                purgedCount++;
            }

            return string.Empty;
        }

        private (int index, int position) FindDeletedRecord()
        {
            using var fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream, Encoding.Default);

            while (fileStream.Position < fileStream.Length)
            {
                fileStream.Position += MaxRecordLength - sizeof(bool);
                if (binaryReader.ReadBoolean())
                {
                    fileStream.Position -= MaxRecordLength - sizeof(bool);
                    return (binaryReader.ReadInt32(), (int)(fileStream.Position - sizeof(int)));
                }
            }

            return (-1, -1);
        }

        private (int index, int position) FindAliveRecord()
        {
            using var fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream, Encoding.Default);
            fileStream.Position = fileStream.Length - 1;
            while (fileStream.Position > 0)
            {
                fileStream.Position -= sizeof(bool);
                if (binaryReader.ReadBoolean() == false)
                {
                    fileStream.Position -= MaxRecordLength - sizeof(bool);
                    return (binaryReader.ReadInt32(), (int)(fileStream.Position - sizeof(int)));
                }

                fileStream.Position -= MaxRecordLength;
            }

            return (-1, -1);
        }

        private int FindNextIndex()
        {
            using var fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream, Encoding.Default);
            var recordBuffer = new byte[MaxRecordLength];

            while (fileStream.Position < fileStream.Length)
            {
                fileStream.Read(recordBuffer);
                if (recordBuffer[^1] == (byte)RecordState.Deleted)
                {
                    fileStream.Position -= MaxRecordLength;
                    return binaryReader.ReadInt32();
                }
            }

            return this.idOffsetDictionary.Count > 0 ? this.GenerateNewIndex() : 1;
        }

        private int GenerateNewIndex()
        {
            var result = Enumerable.Range(1, this.idOffsetDictionary.Keys.Max()).Except(this.idOffsetDictionary.Keys)
                .FirstOrDefault();

            return result == default ? this.idOffsetDictionary.Keys.Max() + 1 : result;
        }

        private (FileCabinetRecord record, RecordState state, int position) TryFindRecordById(int id)
        {
            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);
            using BinaryReader reader = new BinaryReader(fileStream, Encoding.Default);
            byte[] recordBuffer = new byte[MaxRecordLength];
            FileCabinetRecord record;

            if (this.idOffsetDictionary.ContainsKey(id))
            {
                fileStream.Position = this.idOffsetDictionary[id];
                fileStream.Read(recordBuffer);
                record = FromByteToRecord(recordBuffer, out RecordState state);
                return (record, state, this.idOffsetDictionary[id]);
            }

            while (fileStream.Position < fileStream.Length)
            {
                if (reader.ReadInt32() == id)
                {
                    fileStream.Position -= sizeof(int);
                    fileStream.Read(recordBuffer);
                    record = FromByteToRecord(recordBuffer, out RecordState state);
                    return (null, state, (int)(fileStream.Position - MaxRecordLength));
                }

                fileStream.Position += MaxRecordLength - sizeof(int);
            }

            return (null, RecordState.Deleted, -1);
        }

        private void AnalyseDatabase()
        {
            var recordBuffer = new byte[MaxRecordLength];
            using FileStream fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Read);

            while (fileStream.Position < fileStream.Length)
            {
                fileStream.Read(recordBuffer);
                var record = FromByteToRecord(recordBuffer, out RecordState state);
                int position = (int)(fileStream.Position - MaxRecordLength);

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
                this.dateTimeOffsetDictionary[record.DateOfBirth] = new List<int>(position);
            }

            if (this.firstNameOffsetDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameOffsetDictionary[record.FirstName].Add(position);
            }
            else
            {
                this.firstNameOffsetDictionary[record.FirstName] = new List<int>(position);
            }

            if (this.lastNameOffsetDictionary.ContainsKey(record.LastName))
            {
                this.lastNameOffsetDictionary[record.LastName].Add(position);
            }
            else
            {
                this.lastNameOffsetDictionary[record.LastName] = new List<int>(position);
            }

            this.idOffsetDictionary[record.Id] = record.Id;
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
            using MemoryStream memStream = new MemoryStream(bytes);
            using BinaryReader binaryReader = new BinaryReader(memStream);

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
            var textASCII = Encoding.ASCII.GetBytes(text);
            int textLength = text.Length;

            if (textLength > MaxStringLength)
            {
                textLength = MaxStringLength;
            }

            Array.Copy(textASCII, result, textLength);
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
    }
}
