// <copyright file="RecordGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Linq;
    using FileCabinetApp;

    /// <summary>
    /// Generate <see cref="FileCabinetRecord"/> instances.
    /// </summary>
    public static class RecordGenerator
    {
        private static readonly Random Random = new ();

        /// <summary>
        /// Generate <see cref="FileCabinetRecord"/> instances by usind <see cref="System.Random"/> and <see cref="Guid"/> classes for generate data.
        /// </summary>
        /// <param name="startId">Start id for the first record in the iteration.</param>
        /// <param name="recordAmount">Count of records to generate.</param>
        /// <returns>Array of generated <see cref="FileCabinetRecord"/>'s.</returns>
        public static FileCabinetRecord[] GenerateRecord(int startId, int recordAmount)
        {
            var resultRecordArray = new FileCabinetRecord[recordAmount];
            char[] validGenderValue = new char[] { 'f', 'F', 'M', 'm' };

            for (int recordCount = 0; recordCount < recordAmount; recordCount++)
            {
                int id = startId++;
                string firstName = new (Guid.NewGuid().ToString().Where(symb => !char.IsDigit(symb) && !char.IsPunctuation(symb)).ToArray());
                string lastName = new (Guid.NewGuid().ToString().Where(symb => !char.IsDigit(symb) && !char.IsPunctuation(symb)).ToArray());
                DateTime dateOfBirth = DateTime.Parse($"{Random.Next(1, 28)}.{Random.Next(1, 12)}.{Random.Next(1951, DateTime.Now.Year)}", Program.Culture);
                short height = (short)Random.Next(0);
                decimal money = Random.Next(0, int.MaxValue);
                char gender = validGenderValue[Random.Next(0, validGenderValue.Length)];

                resultRecordArray[recordCount] = new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Height = height, Money = money, Gender = gender };
            }

            return resultRecordArray;
        }
    }
}
