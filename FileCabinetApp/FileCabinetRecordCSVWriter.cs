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

        }
    }
}
