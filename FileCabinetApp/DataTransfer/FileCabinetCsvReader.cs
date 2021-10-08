using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.DataTransfer
{
    /// <summary>
    /// Reads records from a CSV file.
    /// </summary>
    public sealed class FileCabinetCsvReader : IRecordDataLoader, IDisposable
    {
        private const int IdIndex = 0;
        private const int FirstNameIndex = 1;
        private const int LastNameIndex = 2;
        private const int DateOfBirthIndex = 3;
        private const int HeightIndex = 4;
        private const int MoneyIndex = 5;
        private const int GenderIndex = 6;
        private const int FieldsCount = 7;

        private readonly string filepath;
        private TextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCsvReader"/> class.
        /// </summary>
        /// <param name="reader">Destination file stream.</param>
        public FileCabinetCsvReader(TextReader reader) => this.reader = reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCsvReader"/> class and save path to destination file.
        /// </summary>
        /// <param name="filePath">Destination file path.</param>
        public FileCabinetCsvReader(string filePath) => this.filepath =
            filePath ?? throw new ArgumentNullException(nameof(filePath), "File path can't be null");

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetCsvReader"/> class.
        /// </summary>
        ~FileCabinetCsvReader()
        {
            this.Dispose(false);
        }

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

            this.reader.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) => this.reader?.Dispose();
    }
}
