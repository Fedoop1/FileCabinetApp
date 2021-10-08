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
        private static readonly char[] ValidGenderValue = { 'f', 'F', 'M', 'm' };
        private static readonly string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly Random Random = new ();

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
                    DateOfBirth = new DateTime(Random.Next(1950, DateTime.Now.Year), Random.Next(1, 13), Random.Next(1, 31)),
                    Height = (short)Random.Next(0, 250),
                    Money = Random.Next(0, int.MaxValue),
                    Gender = ValidGenderValue[Random.Next(0, ValidGenderValue.Length)],
                };
            }
        }

        private static string GenerateString()
        {
            char[] word = new char[Random.Next(0, 50)];

            for (int index = 0; index < word.Length; index++)
            {
                word[index] = Chars[Random.Next(Chars.Length)];
            }

            return new string(word);
        }
    }
}
