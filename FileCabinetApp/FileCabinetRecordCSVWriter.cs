using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordCSVWriter
    {
        private TextWriter writer;

        public FileCabinetRecordCSVWriter(TextWriter writer)
        {
            this.writer = writer;
        }

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
