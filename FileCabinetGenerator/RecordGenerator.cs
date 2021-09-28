using System;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Generate <see cref="FileCabinetRecord"/> instances.
    /// </summary>
    public static class RecordGenerator
    {
        private static readonly char[] validGenderValue = new[] { 'f', 'F', 'M', 'm' };
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random random = new ();

        /// <summary>
        /// Generate <see cref="FileCabinetRecord"/> instances by use <see cref="System.Random"/> and <see cref="Guid"/> classes for generate data.
        /// </summary>
        /// <param name="startId">Start id for the first record in the iteration.</param>
        /// <param name="recordAmount">Count of records to generate.</param>
        /// <returns>Array of generated <see cref="FileCabinetRecord"/>'s.</returns>
        public static FileCabinetRecord[] GenerateRecord(int startId, int recordAmount)
        {
            var resultRecordArray = new FileCabinetRecord[recordAmount];

            for (int recordCount = 0; recordCount < recordAmount; recordCount++)
            {
                resultRecordArray[recordCount] = new FileCabinetRecord()
                {
                    Id = startId++,
                    FirstName = GenerateString(),
                    LastName = GenerateString(),
                    DateOfBirth = new DateTime(random.Next(31), random.Next(13), random.Next(DateTime.Now.Year)),
                    Height = (short)random.Next(),
                    Money = random.Next(0, int.MaxValue),
                    Gender = validGenderValue[random.Next(0, validGenderValue.Length)],
                };
            }

            return resultRecordArray;
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
