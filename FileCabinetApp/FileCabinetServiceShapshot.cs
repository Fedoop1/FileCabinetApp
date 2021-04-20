using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetServiceShapshot
    {
        private FileCabinetRecord[] fileCabinetRecordsArray;

        public FileCabinetServiceShapshot(FileCabinetRecord[] fileCabinetRecords)
        {
            this.fileCabinetRecordsArray = fileCabinetRecords;
        }

        public void SaveToCSV(StreamWriter writer)
        {
            var fileCabinetRecordCSVWriter = new FileCabinetRecordCSVWriter(writer);
            fileCabinetRecordCSVWriter.Write(this.fileCabinetRecordsArray);
        }
    }
}
