using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Create CSV document based on records array data, serialize and save it to disk.
    /// </summary>
    public class FileCabinetRecordCSVWriter
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCSVWriter"/> class.
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/> with file path and other additionфд settings.</param>
        public FileCabinetRecordCSVWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Wtite <see cref="FileCabinetRecord"/> array data into CSV file and save it.
        /// </summary>
        /// <param name="fileCabinetRecordsArray">Contains actual <see cref="FileCabinetRecord"/> data.</param>
        public void Write(FileCabinetRecord[] fileCabinetRecordsArray)
        {
            if (fileCabinetRecordsArray?.Length == 0)
            {
                return;
            }

            // Id,First Name,Last Name,Date of Birth,...
            foreach (var record in fileCabinetRecordsArray)
            {
                this.writer.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth.ToShortDateString()},{record.Height},{record.Money},{record.Gender}.");
            }
        }
    }
}
