using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FileCabinetApp.Interfaces;

#pragma warning disable SA1116 // Split parameters should start on line after declaration

namespace FileCabinetApp.DataTransfer
{
    /// <summary>
    /// Serialize <see cref="FileCabinetRecord"/> data and save it to destination file.
    /// </summary>
    public sealed class FileCabinetRecordXmlLWriter : IRecordDataSaver, IDisposable
    {
        private readonly string filepath;
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlLWriter"/> class.
        /// </summary>
        /// <param name="writer">Stream to destination file.</param>
        public FileCabinetRecordXmlLWriter(TextWriter writer) => this.writer = writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlLWriter"/> class.
        /// </summary>
        /// <param name="filepath">Destination file path.</param>
        public FileCabinetRecordXmlLWriter(string filepath) => this.filepath =
            filepath ?? throw new ArgumentNullException(nameof(filepath), "File path can't be null");

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetRecordXmlLWriter"/> class.
        /// </summary>
        ~FileCabinetRecordXmlLWriter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Create and Write <see cref="FileCabinetRecord"/> source to XML format and save it to destination file.
        /// </summary>
        /// <param name="source">Array of <see cref="FileCabinetRecord"/>.</param>
        /// <param name="append">Flag which indicate if file will appended or rewritten.</param>
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
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing) => this.writer?.Dispose();
    }
}
