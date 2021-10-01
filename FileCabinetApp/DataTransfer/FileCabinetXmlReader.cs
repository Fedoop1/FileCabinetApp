using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.DataTransfer
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
            this.reader ??= new StreamReader(this.filepath);
            var document = XDocument.Load(this.reader);
            foreach (var record in document.XPathSelectElements("Records/Record"))
            {
                yield return new FileCabinetRecord()
                {
                    Id = int.Parse(record.Attribute("Id").Value, CultureInfo.CurrentCulture),
                    FirstName = record.XPathSelectElement("Name").Attribute("First").Value,
                    LastName = record.XPathSelectElement("Name").Attribute("Last").Value,
                    DateOfBirth = DateTime.Parse(record.XPathSelectElement("DateOfBirth").Value, CultureInfo.CurrentCulture),
                    Height = short.Parse(record.XPathSelectElement("Height").Value, CultureInfo.CurrentCulture),
                    Gender = char.Parse(record.XPathSelectElement("Gender").Value),
                    Money = decimal.Parse(record.XPathSelectElement("Money").Value, CultureInfo.CurrentCulture),
                };
            }
        }
    }
}
