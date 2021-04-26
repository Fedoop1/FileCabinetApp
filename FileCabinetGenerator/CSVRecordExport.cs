namespace FileCabinetApp
{
    using System;
    using System.IO;
    using System.Text;

    public static class CSVRecordExport
    {
        public static void Export(string filePath, FileCabinetRecord[] recordArray)
        {
            if (recordArray is null)
            {
                throw new ArgumentNullException(nameof(recordArray), "Array of records is null");
            }

            using (StreamWriter textWriter = new StreamWriter(filePath, false, Encoding.Default))
            {
                foreach (var record in recordArray)
                {
                    textWriter.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.Height},{record.Money},{record.Gender}");
                }
            }
        }
    }
}
