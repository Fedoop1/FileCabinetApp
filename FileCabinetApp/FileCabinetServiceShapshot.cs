using System.IO;

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

        public void SaveToXML(StreamWriter writer)
        {
            var fileCabinetRecordXMLWriter = new FileCabinetRecordXMLWriter(writer);
            fileCabinetRecordXMLWriter.Write(this.fileCabinetRecordsArray);
        }
    }
}
