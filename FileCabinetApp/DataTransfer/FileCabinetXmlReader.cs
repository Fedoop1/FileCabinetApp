using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for deserializing <see cref="FileCabinetRecord"/> information from XML file.
    /// </summary>
    public class FileCabinetXMLReader : IRecordDataLoader
    {
        private readonly string filepath;
        private TextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXMLReader"/> class.
        /// </summary>
        /// <param name="reader"><see cref="TextReader"/> with information about XML format file.</param>
        public FileCabinetXMLReader(TextReader reader) => this.reader = reader ?? throw new ArgumentNullException(nameof(reader), "Reader can't be null");

        public FileCabinetXMLReader(string filepath) => this.filepath =
            filepath ?? throw new ArgumentNullException(nameof(filepath), "File path can't be null");

        /// <summary>
        /// Read and deserialize all information from file and return it <see cref="IList{FileCabinetRecord}"/> representation.
        /// </summary>
        /// <returns><see cref="IList{FileCabinetRecord}"/> representation of records into XML file.</returns>
        public IEnumerable<FileCabinetRecord> Load()
        {
            // TODO: Change implementation to XPath navigation and parsing XML Document
            this.reader ??= new StreamReader(this.filepath);
            var xmlSerializer = new XmlSerializer(typeof(FileCabinetRecord[]));
            return xmlSerializer.Deserialize(this.reader) is FileCabinetRecord[] records ? records : throw new ArgumentException($"Xml file doesn't contain data to {nameof(FileCabinetRecord)} type.");
        }
    }
}
