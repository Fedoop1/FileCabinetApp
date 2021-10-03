using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class which convert <see cref="FileCabinetRecord"/> data to XML format and write it to destination file.
    /// </summary>
    public class XmlRecordExporter : IRecordExporter
    {
        private readonly string filepath;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRecordExporter"/> class.
        /// </summary>
        /// <param name="filepath">Destination file path.</param>
        /// <exception cref="ArgumentNullException">Throws when file path is null.</exception>
        public XmlRecordExporter(string filepath) => this.filepath =
            filepath ?? throw new ArgumentNullException(nameof(filepath), "Destination file path is null");

        /// <summary>
        /// Exports source to destination file in XML format.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentNullException">Throws when source is null.</exception>
        public void Export(IEnumerable<FileCabinetRecord> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Records source is null");
            }

            using var writer  = new StreamWriter(this.filepath, false);

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

            document.Save(writer);
        }
    }
}
