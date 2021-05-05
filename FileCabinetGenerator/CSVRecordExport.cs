// <copyright file="CSVRecordExport.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Static class which export data array into CSV file.
    /// </summary>
    public static class CSVRecordExport
    {
        /// <summary>
        /// Export <see cref="FileCabinetRecord"/> array into CSV format.
        /// </summary>
        /// <param name="fileStream"><see cref="FileStream"/> with information about file for exporting.</param>
        /// <param name="recordArray">Array of <see cref="FileCabinetRecord"/> with information about records.</param>
        public static void Export(FileStream fileStream, FileCabinetRecord[] recordArray)
        {
            if (recordArray is null)
            {
                throw new ArgumentNullException(nameof(recordArray), "Array of records is null");
            }

            using var textWriter = new StreamWriter(fileStream);

            foreach (var record in recordArray)
            {
                textWriter.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.Height},{record.Money},{record.Gender}");
            }
        }
    }
}
