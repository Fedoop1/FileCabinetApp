using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.RecordPrinters
{
    public class DefaultPrinter : IRecordPrinter
    {
        private readonly TextWriter destinationStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPrinter"/> class.
        /// </summary>
        /// <param name="destinationStream">Destination stream.</param>
        /// <exception cref="ArgumentNullException">Throws when destination source is null.</exception>
        public DefaultPrinter(TextWriter destinationStream) => this.destinationStream = destinationStream ??
            throw new ArgumentNullException(nameof(destinationStream), "Destination stream can't be null");

        /// <summary>
        /// Prints source to destination file with using specified select method according to selected fields in ToString() format.
        /// </summary>
        /// <param name="source">Records source.</param>
        /// <param name="selectedFields">Collection of selected fields.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when source is null
        /// or
        /// Collection of selected fields is null.
        /// </exception>
        public void Print(IEnumerable<FileCabinetRecord> source, IEnumerable<PropertyInfo> selectedFields)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Source can't be null");
            }

            if (selectedFields is null)
            {
                throw new ArgumentNullException(nameof(source), "Source of selected fields can't be null");
            }

            foreach (var record in source)
            {
                foreach (var field in selectedFields)
                {
                    this.destinationStream.Write($"{field.Name}: {field.GetValue(record)}; ");
                }

                this.destinationStream.Write('\n');
            }
        }
    }
}
