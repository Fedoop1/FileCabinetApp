using System;
using System.Collections.Generic;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Generate <see cref="FileCabinetRecord"/> instances.
    /// </summary>
    public static class RecordGenerator
    {
        private static readonly char[] validGenderValue = { 'f', 'F', 'M', 'm' };
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly Random random = new ();

        /// <summary>
        /// Generate <see cref="FileCabinetRecord"/> instances.
        /// </summary>
        /// <returns>Sequence of generated <see cref="FileCabinetRecord"/>'s.</returns>
        public static IEnumerable<FileCabinetRecord> GenerateRecord(GenerationSettings settings)
        {

            for (var recordCount = 0; recordCount < settings.RecordsAmount; recordCount++)
            {
                yield return new FileCabinetRecord()
                {
                    Id = settings.StartId++,
                    FirstName = GenerateString(),
                    LastName = GenerateString(),
                    DateOfBirth = new DateTime(random.Next(1950, DateTime.Now.Year), random.Next(1, 13), random.Next(1, 31)),
                    Height = (short)random.Next(0, 250),
                    Money = random.Next(0, int.MaxValue),
                    Gender = validGenderValue[random.Next(0, validGenderValue.Length)],
                };
            }
        }

        private static string GenerateString()
        {
            char[] word = new char[random.Next(0, 50)];

            for (int index = 0; index < word.Length; index++)
            {
                word[index] = chars[random.Next(chars.Length)];
            }

            return new string(word);
        }
    }
}
