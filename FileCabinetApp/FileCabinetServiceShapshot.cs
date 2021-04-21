using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot class with private information about list of <see cref="FileCabinetRecord"/> required for secure serialization.
    /// </summary>
    public class FileCabinetServiceShapshot
    {
        private FileCabinetRecord[] fileCabinetRecordsArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceShapshot"/> class and save information and assign inner field by reference to an array with records.
        /// </summary>
        /// <param name="fileCabinetRecords"><see cref="FileCabinetRecord"/> array with data about records.</param>
        public FileCabinetServiceShapshot(FileCabinetRecord[] fileCabinetRecords)
        {
            this.fileCabinetRecordsArray = fileCabinetRecords;
        }

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetRecordCSVWriter"/> and invoke Write method.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"/> with information about file path and addition settings.</param>
        public void SaveToCSV(StreamWriter writer)
        {
            var fileCabinetRecordCSVWriter = new FileCabinetRecordCSVWriter(writer);
            fileCabinetRecordCSVWriter.Write(this.fileCabinetRecordsArray);
        }

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetRecordXMLWriter"/> and invoke Write method.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"/> with information about file path and addition settings.</param>
        public void SaveToXML(StreamWriter writer)
        {
            var fileCabinetRecordXMLWriter = new FileCabinetRecordXMLWriter(writer);
            fileCabinetRecordXMLWriter.Write(this.fileCabinetRecordsArray);
        }
    }
}
