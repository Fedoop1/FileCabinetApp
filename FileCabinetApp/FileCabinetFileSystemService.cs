using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Organizes work through a <see cref="FileStream"/> in a byte format. Is an alternative of <see cref="FileCabinetMemoryService"/>.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IRecordValidator
    {
        /// <summary>
        /// Max record length of 1 record. Include all fields from <see cref="FileCabinetRecord"/> and reserved bytes.
        /// </summary>
        public const int MaxRecordLength = 255 + sizeof(short) + sizeof(decimal) + sizeof(char) + sizeof(bool);

        /// <summary>
        /// Accepted constant to describe the maximum length of a string field in byte format.
        /// </summary>
        public const int MaxNameLength = 120;

        private readonly FileStream fileStream;
        private int deletedRecordsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class and set the file stream instance.
        /// </summary>
        /// <param name="fileStream">File stream instance wich organize work with external storage.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream ?? new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>
        /// Gets actual length of .db file into <see cref="FileStream"/>.
        /// </summary>
        /// <value>
        /// Actual length of .db file into file stream.
        /// </value>
        public int FileLength => (int)this.fileStream.Length;

        /// <summary>
        /// Gets count of records into <see cref="FileStream"/>.
        /// </summary>
        /// <value>Actual records count.</value>
        public int RecordsCount => this.GetCountOfRecords();

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecordData recordData)
        {
            this.ValidateParameters(recordData);

            var record = new FileCabinetRecord
            {
                Id = FindFreeIndex(),
                FirstName = recordData?.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Height = recordData.Height,
                Money = recordData.Money,
                Gender = recordData.Gender,
            };

            byte[] recordByteArray = RecordToByteConverter(record);
            this.fileStream.Write(recordByteArray, 0, MaxRecordLength);

            Console.WriteLine("File length is: " + this.FileLength);
            return record.Id;

            int FindFreeIndex()
            {
                const int Deleted = 1;
                const int IdIndex = 0;
                this.fileStream.Position = 0;
                var record = new byte[MaxRecordLength];

                while (this.fileStream.Position != this.FileLength)
                {
                    this.fileStream.Read(record);
                    if (record[MaxRecordLength - 1] == Deleted && !this.FindRecord(record[IdIndex]).isExist)
                    {
                        return record[IdIndex];
                    }
                }

                return this.RecordsCount + 1;
            }
        }

        /// <inheritdoc/>
        public IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecordData recordData)
        {
            var recordIndex = this.FindRecord(id);

            if (!recordIndex.isExist)
            {
                Console.WriteLine("Record doesn't exist.");
                return;
            }

            this.ValidateParameters(recordData);

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = recordData?.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Height = recordData.Height,
                Money = recordData.Money,
                Gender = recordData.Gender,
            };

            byte[] recordByteArray = RecordToByteConverter(record);

            this.SetPositionOnRecord(recordIndex.fileRecordIndex);
            this.fileStream.Write(recordByteArray);

            Console.WriteLine($"Record {id} successfull update.");
        }

        

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByDayOfBirth(string dateOfBirth)
        {
            if (!DateTime.TryParse(dateOfBirth, out DateTime birthDate))
            {
                Console.WriteLine("Date of birth is incorrect!");
                return Array.Empty<FileCabinetRecord>();
            }

            ReadOnlyCollection<FileCabinetRecord> recordCollection = this.GetRecords();
            return recordCollection.Where(record => record.DateOfBirth == birthDate).ToArray();
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                Console.WriteLine("First name is incorrect!");
                return Array.Empty<FileCabinetRecord>();
            }

            ReadOnlyCollection<FileCabinetRecord> recordCollection = this.GetRecords();
            return recordCollection.Where(record => record.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("First name is incorrect!");
                return Array.Empty<FileCabinetRecord>();
            }

            ReadOnlyCollection<FileCabinetRecord> recordCollection = this.GetRecords();
            return recordCollection.Where(record => record.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> recordList = new List<FileCabinetRecord>();
            this.fileStream.Position = 0;

            while (this.fileStream.Position != this.FileLength)
            {
                byte[] recordByteArray = new byte[MaxRecordLength];

                using (var memoryStream = new MemoryStream(recordByteArray))
                using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
                {
                    int recordId = binaryReader.ReadInt32();
                    var byteName = binaryReader.ReadBytes(MaxNameLength);
                    string firstName = ASCIIToString(byteName);
                    var byteLastName = binaryReader.ReadBytes(MaxNameLength);
                    string lastName = ASCIIToString(byteLastName);
                    int dateOfBirthDay = binaryReader.ReadInt32();
                    int dateOfBirthMonth = binaryReader.ReadInt32();
                    int dateOfBirthYear = binaryReader.ReadInt32();
                    short height = binaryReader.ReadInt16();
                    decimal money = binaryReader.ReadDecimal();
                    char gender = binaryReader.ReadChar();
                    bool isDeleted = binaryReader.ReadBoolean();

                    if (!DateTime.TryParse($"{dateOfBirthDay}.{dateOfBirthMonth}.{dateOfBirthYear}", out DateTime dateOfBirth))
                    {
                        throw new ArgumentException("Date of birth is incorrect.");
                    }

                    var record = new FileCabinetRecord()
                    {
                        Id = recordId,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        Height = height,
                        Money = money,
                        Gender = gender,
                    };

                    if (!isDeleted)
                    {
                        recordList.Add(record);
                    }
                }
            }

            return recordList.AsReadOnly();
        }

        /// <inheritdoc/>
        public (int actualRecords, int deletedRecords) GetStat()
        {
            return (this.RecordsCount, this.deletedRecordsCount);
        }

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetServiceShapshot"/> wich contain all information about exists records.
        /// </summary>
        /// <returns>Instance of <see cref="FileCabinetServiceShapshot"/>.</returns>
        public FileCabinetServiceShapshot MakeSnapshot()
        {
            var recordsArray = this.GetRecords().ToArray();
            return new FileCabinetServiceShapshot(recordsArray);
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            this.CreateValidator().ValidateParameters(recordData);
        }

        /// <summary>
        /// Releases resources from under the <see cref="FileCabinetFilesystemService"/> and releases the flow of <see cref="FileStream"/>.
        /// </summary>
        public void Dispose()
        {
            this.fileStream.Close();
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceShapshot restoreSnapshot)
        {
            if (restoreSnapshot is null)
            {
                throw new ArgumentNullException(nameof(restoreSnapshot), "Restore snapshot is null");
            }

            var restoreRecordsList = restoreSnapshot.Records;
            var recordList = this.GetRecords();

            foreach (var restoreRecord in restoreRecordsList)
            {
                byte[] recordByteArray = RecordToByteConverter(restoreRecord);

                if (recordList.Any(record => record.Id == restoreRecord.Id))
                {
                    this.SetPositionOnRecord(restoreRecord.Id);
                    this.fileStream.Write(recordByteArray);
                }
                else
                {
                    this.fileStream.Write(recordByteArray, 0, MaxRecordLength);
                }
            }
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int index)
        {
            const byte Deleted = 1;
            byte[] record = new byte[MaxRecordLength];

            var recordIndex = this.FindRecord(index);

            if (!recordIndex.isExist)
            {
                return false;
            }

            this.SetPositionOnRecord(recordIndex.fileRecordIndex);
            this.fileStream.Read(record);
            record[MaxRecordLength - 1] = Deleted;
            this.SetPositionOnRecord(recordIndex.fileRecordIndex);
            this.fileStream.Write(record);
            this.deletedRecordsCount += 1;
            return true;
        }

        /// <inheritdoc/>
        public string Purge()
        {
            int allRecords = this.FileLength / MaxRecordLength;
            byte[][] recordArray = new byte[this.FileLength / MaxRecordLength][];

            this.fileStream.Position = 0;
            RecordsToJaggedArray(ref recordArray);
            SortJaggedArray(recordArray);

            return $"Data file processing is completed: {allRecords - this.RecordsCount} of {allRecords} records were purged.";

            void RecordsToJaggedArray(ref byte[][] recordArray)
            {
                using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
                {
                    for (int recordCount = 0; this.fileStream.Position < this.FileLength; recordCount++)
                    {
                        this.SetPositionOnRecord(recordCount + 1);
                        byte[] record = binaryReader.ReadBytes(MaxRecordLength);
                        recordArray[recordCount] = record;
                    }
                }
            }

            void SortJaggedArray(byte[][] recordArray)
            {
                const int NotDeleted = 0;
                const int Deleted = 1;

                for (int firstRecordIndex = 0; firstRecordIndex < recordArray.Length; firstRecordIndex++)
                {
                    if (recordArray[firstRecordIndex][MaxRecordLength - 1] == Deleted)
                    {
                        for (int secondRecordIndex = firstRecordIndex + 1; secondRecordIndex < recordArray.Length; secondRecordIndex++)
                        {
                            if (recordArray[secondRecordIndex][MaxRecordLength - 1] == NotDeleted)
                            {
                                Swap(ref recordArray[firstRecordIndex], ref recordArray[secondRecordIndex]);
                                break;
                            }
                        }
                    }
                }

                void Swap(ref byte[] firstArray, ref byte[] secondArray)
                {
                    var temp = firstArray;
                    firstArray = secondArray;
                    secondArray = temp;
                }
            }
        }

        /// <summary>
        /// Convert <see cref="FileCabinetRecord"/> to byte representation.
        /// </summary>
        /// <param name="record">Record for conversion.</param>
        /// <returns>The byte array with byte representaion of record.</returns>
        private static byte[] RecordToByteConverter(FileCabinetRecord record)
        {
            byte[] recordByteArray = new byte[MaxRecordLength];

            using (var memoryStream = new MemoryStream(recordByteArray))
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(record.Id);
                bool isDeleted = false;

                var byteName = StringToASCII(record.FirstName);
                var byteLastName = StringToASCII(record.LastName);

                binaryWriter.Write(byteName);
                binaryWriter.Write(byteLastName);
                binaryWriter.Write(record.DateOfBirth.Day);
                binaryWriter.Write(record.DateOfBirth.Month);
                binaryWriter.Write(record.DateOfBirth.Year);
                binaryWriter.Write(record.Height);
                binaryWriter.Write(record.Money);
                binaryWriter.Write(record.Gender);
                binaryWriter.Write(isDeleted);

                return recordByteArray;
            }
        }

        /// <summary>
        /// Convert send text to ASCII representation.
        /// </summary>
        /// <param name="text">Text to convert.</param>
        /// <returns>Byte array with text converted to ACKII.</returns>
        private static byte[] StringToASCII(string text)
        {
            var result = new byte[MaxNameLength];
            var textASCII = Encoding.ASCII.GetBytes(text);
            int textLength = text.Length;

            if (textLength > MaxNameLength)
            {
                textLength = MaxNameLength;
            }

            Array.Copy(textASCII, result, textLength);
            return result;
        }

        /// <summary>
        /// Convert ASCII byte array to string representation.
        /// </summary>
        /// <param name="byteName">Byte array of ASCII to convert.</param>
        /// <returns>String result.</returns>
        private static string ASCIIToString(byte[] byteName)
        {
            var result = Encoding.ASCII.GetString(byteName);
            return result.TrimEnd('\0');
        }

        /// <summary>
        /// Return the actual count of records.
        /// </summary>
        /// <returns>Count of records.</returns>
        private int GetCountOfRecords()
        {
            int result = 0;
            const int Deleted = 1;
            this.fileStream.Position = 0;
            var record = new byte[MaxRecordLength];

            while (this.fileStream.Position != this.FileLength)
            {
                this.fileStream.Read(record);
                if (record[MaxRecordLength - 1] == Deleted)
                {
                    continue;
                }

                result++;
            }

            return result;
        }

        /// <summary>
        /// Set position in <see cref="FileStream"/> at selected record.
        /// </summary>
        /// <param name="index">The record to set position.</param>
        private void SetPositionOnRecord(int index)
        {
            this.fileStream.Position = (index - 1) * MaxRecordLength;
        }

        /// <summary>
        /// Try to find record with selected Id in <see cref="FileStream"/>.
        /// </summary>
        /// <param name="id">Id of record to search.</param>
        /// <returns>The tuple of searching result and record position in file.</returns>
        private (bool isExist, int fileRecordIndex) FindRecord(int id)
        {
            const int NotDeleted = 0;
            int fileIndex = 1;
            var record = new byte[MaxRecordLength];

            this.fileStream.Position = 0;
            while (this.fileStream.Position < this.fileStream.Length)
            {
                this.fileStream.Read(record);

                if (id == ByteToIntConvert(record[0..4]) && record[MaxRecordLength - 1] == NotDeleted)
                {
                    return (true, fileIndex);
                }

                fileIndex++;
            }

            return (false, -1);

            int ByteToIntConvert(byte[] bytesArray)
            {
                using (var memoryStream = new MemoryStream(bytesArray))
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.Default, true))
                {
                    return binaryReader.ReadInt32();
                }
            }
        }
    }
}
