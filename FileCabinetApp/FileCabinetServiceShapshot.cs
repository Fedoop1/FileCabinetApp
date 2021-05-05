using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot class with private information about list of <see cref="FileCabinetRecord"/> required for secure serialization.
    /// </summary>
    public class FileCabinetServiceShapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceShapshot"/> class and save information and assign inner field by reference to an array with records.
        /// </summary>
        /// <param name="fileCabinetRecords"><see cref="FileCabinetRecord"/> array with data about records.</param>
        public FileCabinetServiceShapshot(FileCabinetRecord[] fileCabinetRecords)
        {
            this.records = fileCabinetRecords;
        }

        /// <summary>
        /// Gets <see cref="ReadOnlyCollection{T}"/> of <see cref="FileCabinetRecord"/> which is stored into snapshot.
        /// </summary>
        /// <value>
        /// Read only collection of <see cref="FileCabinetRecord"/>.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records => this.records.ToList().AsReadOnly();

        /// <summary>
        /// Load data from CSV file and deserialize it to <see cref="FileCabinetRecord"/> representation.
        /// </summary>
        /// <param name="streamReader">IO Flow with information about data storage file.</param>
        public void LoadFromCSV(StreamReader streamReader)
        {
            var csvReader = new FileCabinetCSVReader(streamReader);
            this.records = csvReader.ReadAll().ToArray();
        }

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetRecordCSVWriter"/> and invoke Write method.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"/> with information about file path and addition settings.</param>
        public void SaveToCSV(StreamWriter writer)
        {
            var fileCabinetRecordCSVWriter = new FileCabinetRecordCSVWriter(writer);
            fileCabinetRecordCSVWriter.Write(this.records);
        }

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetRecordXMLWriter"/> and invoke Write method.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"/> with information about file path and addition settings.</param>
        public void SaveToXML(StreamWriter writer)
        {
            var fileCabinetRecordXMLWriter = new FileCabinetRecordXMLWriter(writer);
            fileCabinetRecordXMLWriter.Write(this.records);
        }

        /// <summary>
        /// Load data from XML file and deserialize it to <see cref="FileCabinetRecord"/> representation.
        /// </summary>
        /// <param name="fileStream">IO Flow with information about data storage file.</param>
        public void LoadFromXML(FileStream fileStream)
        {
            var fileCabinetXMLReader = new FileCabinetXMLReader(fileStream);
            this.records = fileCabinetXMLReader.ReadAll().ToArray();
        }
    }
}
