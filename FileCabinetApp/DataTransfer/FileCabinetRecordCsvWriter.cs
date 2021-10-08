using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.DataTransfer
{
    /// <summary>
    /// Create CSV document serialize records data to it.
    /// </summary>
    public sealed class FileCabinetRecordCsvWriter : IRecordDataSaver, IDisposable
    {
        private readonly string filepath;
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/>Stream to destination file.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="filepath">File path to destination file.</param>
        public FileCabinetRecordCsvWriter(string filepath) => this.filepath = filepath ?? throw new ArgumentNullException(nameof(filepath), "File path can't be null");

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        ~FileCabinetRecordCsvWriter() => this.Dispose(false);

        /// <summary>
        /// Write <see cref="FileCabinetRecord"/> sequence to CSV file and save it.
        /// </summary>
        /// <param name="source">Contains actual <see cref="FileCabinetRecord"/> data.</param>
        /// <param name="append">Append if <c>true</c>, otherwise recreate file.</param>
        public void Save(IEnumerable<FileCabinetRecord> source, bool append)
        {
            this.writer ??= new StreamWriter(this.filepath, append);

            foreach (var record in source ?? throw new ArgumentNullException(nameof(source), "Source can't be null"))
            {
                this.writer.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth.ToShortDateString()},{record.Height},{record.Money},{record.Gender}.");
            }

            this.writer.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) => this.writer?.Dispose();
    }
}
