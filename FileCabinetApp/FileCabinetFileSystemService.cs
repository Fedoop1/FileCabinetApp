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
        public const int MaxRecordLength = 255 + sizeof(short) + sizeof(decimal) + sizeof(char);

        /// <summary>
        /// Accepted constant to describe the maximum length of a string field in byte format.
        /// </summary>
        public const int MaxNameLength = 120;

        private readonly FileStream fileStream;

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
        public int RecordsCount => (int)this.fileStream.Length / MaxRecordLength;

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecordData recordData)
        {
            this.ValidateParameters(recordData);

            var record = new FileCabinetRecord
            {
                Id = this.RecordsCount + 1,
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
        }

        /// <inheritdoc/>
        public IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }

        /// <inheritdoc/>
        public void EditRecord(string id, FileCabinetRecordData recordData)
        {
            if (!int.TryParse(id, out int recordId))
            {
                Console.WriteLine("Record doesn't exist.");
                return;
            }
            else if (recordId > RecordsCount)
            {
                Console.WriteLine("Records count lower than Id.");
                return;
            }

            this.ValidateParameters(recordData);

            var record = new FileCabinetRecord
            {
                Id = recordId,
                FirstName = recordData?.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Height = recordData.Height,
                Money = recordData.Money,
                Gender = recordData.Gender,
            };

            byte[] recordByteArray = RecordToByteConverter(record);

            using (var binaryWriter = new BinaryWriter(this.fileStream, Encoding.Default, true))
            {
                this.fileStream.Position = (recordId - 1) * MaxRecordLength;
                this.fileStream.Write(recordByteArray);
            }

            Console.WriteLine($"Record {recordId} successfull update.");
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

                    recordList.Add(record);
                }
            }

            return recordList.AsReadOnly();
        }

        /// <summary>
        /// Returns the count of records.
        /// </summary>
        /// <returns>Records count.</returns>
        public int GetStat()
        {
            return this.RecordsCount;
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

        private static byte[] RecordToByteConverter(FileCabinetRecord record)
        {
            byte[] recordByteArray = new byte[MaxRecordLength];

            using (var memoryStream = new MemoryStream(recordByteArray))
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(record.Id);

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

                return recordByteArray;
            }
        }

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

        private static string ASCIIToString(byte[] byteName)
        {
            var result = Encoding.ASCII.GetString(byteName);
            return result.TrimEnd('\0');
        }

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
                    this.fileStream.Position = (restoreRecord.Id - 1) * MaxRecordLength;
                    this.fileStream.Write(recordByteArray);
                }
                else
                {
                    this.fileStream.Write(recordByteArray, 0, MaxRecordLength);
                }
            }
        }
    }
}
