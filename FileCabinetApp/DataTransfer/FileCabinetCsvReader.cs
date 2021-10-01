using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Reads records from a CSV file.
    /// </summary>
    public class FileCabinetCSVReader : IRecordDataLoader
    {
        private const int IdIndex = 0;
        private const int FirstNameIndex = 1;
        private const int LastNameIndex = 2;
        private const int DateOfBirthIndex = 3;
        private const int HeightIndex = 4;
        private const int MoneyIndex = 5;
        private const int GenderIndex = 6;
        private const int FieldsCount = 7;

        private TextReader reader;
        private readonly string filepath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCSVReader"/> class and assign <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="streamReader"><see cref="TextReader"/> passed in the class.</param>
        public FileCabinetCSVReader(TextReader reader) => this.reader = reader;

        public FileCabinetCSVReader(string filePath) => this.filepath =
            filePath ?? throw new ArgumentNullException(nameof(filePath), "File path can't be null");

        /// <summary>
        /// Read all <see cref="FileCabinetRecord"/> from the CSV file and add it to <see cref="IList{T}"/>.
        /// </summary>
        /// <returns><see cref="IList{T}"/> representation of records in the CSV file.</returns>
        public IEnumerable<FileCabinetRecord> Load()
        {
            this.reader ??= new StreamReader(this.filepath);
            string recordDataLine;

            while ((recordDataLine = this.reader.ReadLine()) != null)
            {
                var recordDataArray = recordDataLine.Split(",", FieldsCount, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (recordDataArray.Length < FieldsCount)
                {
                    throw new ArgumentException($"Invalid record! " + (recordDataArray.Length >= 1 ? $"Record id = {recordDataArray[IdIndex]}" : "Bad record data."));
                }

                if (!int.TryParse(recordDataArray[IdIndex], out int id))
                {
                    throw new ArgumentException($"#{id}: id {recordDataArray[IdIndex]} is incorrect!");
                }

                if (!DateTime.TryParse(recordDataArray[DateOfBirthIndex], out DateTime dateOfBirth))
                {
                    throw new ArgumentException($"#{id}: Date of birth {recordDataArray[DateOfBirthIndex]} is incorrect!");
                }

                if (!short.TryParse(recordDataArray[HeightIndex], out short height))
                {
                    throw new ArgumentException($"#{id}: Height {recordDataArray[HeightIndex]} is incorrect!");
                }

                if (!decimal.TryParse(recordDataArray[MoneyIndex], out decimal money))
                {
                    throw new ArgumentException($"#{id}: Money {recordDataArray[MoneyIndex]} is incorrect!");
                }

                yield return new FileCabinetRecord()
                {
                    Id = id,
                    FirstName = recordDataArray[FirstNameIndex],
                    LastName = recordDataArray[LastNameIndex],
                    DateOfBirth = dateOfBirth,
                    Height = height,
                    Money = money,
                    Gender = recordDataArray[GenderIndex][0],
                };
            }
        }
    }
}
