using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService, IRecordValidator
    {
        // Id(int) + FirstName(string[const]) + LastName(string[const]) + DateOfBirth(int + int + int /4 * 3/) + Height(short) + Money(decimal) + Gender(Char).
        public const int MaxRecordLength = 255 + sizeof(short) + sizeof(decimal) + sizeof(char);
        public const int MaxNameLength = 120;

        public int FileLength => (int)this.fileStream.Length;

        private FileStream fileStream;

        public int RecordsCount => (int)this.fileStream.Length / MaxRecordLength;

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;

        }

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

                this.fileStream.Write(recordByteArray, 0, recordByteArray.Length);
            }

            Console.WriteLine("File length is: " + this.FileLength);
            return record.Id;
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

        /*
            0   short   2   Status  Reserved
            2   int32   4   Id            Record ID
            6   char[]  120 FirstName    First name
            126 char[]  120 LastName     Last name
            246 int32   4   Year     Date of birth
            250 int32   4   Month    Date of birth
            254 int32   4   Day      Date of birth
            270 int16   16  Height          Height
            286 decimal 16  Money            Money
            288 char    2   Gender          Gender
         */

        /// <inheritdoc/>
        public IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }

        /// <inheritdoc/>
        public void EditRecord(string id, FileCabinetRecordData recordData)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByDayOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            throw new NotImplementedException();
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

                    var record = new FileCabinetRecord()
                    {
                        Id = recordId,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = DateTime.Parse($"{dateOfBirthDay}.{dateOfBirthMonth}.{dateOfBirthYear}", System.Globalization.CultureInfo.InvariantCulture),
                        Height = height,
                        Money = money,
                        Gender = gender,
                    };

                    recordList.Add(record);
                }
            }

            return recordList.AsReadOnly();
        }

        private static string ASCIIToString(byte[] byteName)
        {
            var result = Encoding.ASCII.GetString(byteName);
            return result.TrimEnd('\0');
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.RecordsCount;
        }

        /// <inheritdoc/>
        public FileCabinetServiceShapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            this.CreateValidator().ValidateParameters(recordData);
        }

        public void Dispose()
        {
            this.fileStream.Close();
        }
    }
}
