namespace FileCabinetApp
{
    using System;
    using System.IO;
    using System.Text;

    public static class CSVRecordExport
    {
        public static void Export(FileStream fileStream, FileCabinetRecord[] recordArray)
        {
            if (recordArray is null)
            {
                throw new ArgumentNullException(nameof(recordArray), "Array of records is null");
            }

            using (StreamWriter textWriter = new StreamWriter(fileStream))
            {
                foreach (var record in recordArray)
                {
                    textWriter.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.Height},{record.Money},{record.Gender}");
                }
            }
        }
    }
}
