using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Create XMLDocument based on records array data, serialize and save it to disk.
    /// </summary>
    public class FileCabinetRecordXmlLWriter : IRecordDataSaver
    {
        private readonly string filepath;
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlLWriter"/> class and create <see cref="xmlWriter"/> object based on <see cref="StreamWriter"/> argument.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"/> with file path and other additional settings.</param>
        public FileCabinetRecordXmlLWriter(TextWriter writer) => this.writer = writer;

        public FileCabinetRecordXmlLWriter(string filepath) => this.filepath =
            filepath ?? throw new ArgumentNullException(nameof(filepath), "File path can't be null");

        /// <summary>
        /// Create and Write <see cref="FileCabinetRecord"/> source to XML format and save it to destination file.
        /// </summary>
        /// <param name="source">Array of <see cref="FileCabinetRecord"/>.</param>
        public void Save(IEnumerable<FileCabinetRecord> source, bool append)
        {
            this.writer ??= new StreamWriter(this.filepath, append);

            var document = new XElement("Records", source.Select(record =>
                new XElement("Record", 
                    new XAttribute("Id", record.Id),
                    new XElement("Name",
                        new XAttribute("First", record.FirstName),
                        new XAttribute("Last", record.LastName)),
                    new XElement("DateOfBirth", record.DateOfBirth.ToShortDateString()),
                    new XElement("Height", record.Height),
                    new XElement("Gender", record.Gender),
                    new XElement("Money", record.Money))));

            document.Save(this.writer);
        }
    }
}
