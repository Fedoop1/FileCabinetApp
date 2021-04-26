using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetCSVReader
    {
        private StreamReader reader;

        public FileCabinetCSVReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            var result = new List<FileCabinetRecord>();
            string recordDataLine;
            const int IdIndex = 0;
            const int FirstNameIndex = 1;
            const int LastNameIndex = 2;
            const int DateOfBirthIndex = 3;
            const int HeightIndex = 4;
            const int MoneyIndex = 5;
            const int GenderIndex = 6;

            while ((recordDataLine = this.reader.ReadLine()) != null)
            {
                try
                {
                    var recordDataArray = recordDataLine.Split(",", 7);
                    if (!int.TryParse(recordDataArray[IdIndex], out int id))
                    {
                        throw new ArgumentException($"#{id}: id {recordDataArray[0]} is incorrect!");
                    }

                    string firstName = recordDataArray[FirstNameIndex];
                    string lastName = recordDataArray[LastNameIndex];

                    if (!DateTime.TryParse(recordDataArray[DateOfBirthIndex], out DateTime dateOfBirth))
                    {
                        throw new ArgumentException($"#{id}: Date of birth {recordDataArray[3]} is incorrect!");
                    }

                    if (!short.TryParse(recordDataArray[HeightIndex], out short height))
                    {
                        throw new ArgumentException($"#{id}: Height {recordDataArray[4]} is incorrect!");
                    }

                    if (!decimal.TryParse(recordDataArray[MoneyIndex], out decimal money))
                    {
                        throw new ArgumentException($"#{id}: Money {recordDataArray[5]} is incorrect!");
                    }

                    char gender = recordDataArray[GenderIndex][0];

                    result.Add(new FileCabinetRecord()
                    {
                        Id = id,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        Height = height,
                        Money = money,
                        Gender = gender,
                    });
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }

            return result;
        }
    }
}
