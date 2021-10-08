using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

#pragma warning disable SA1116 // Split parameters should start on line after declaration

namespace FileCabinetApp.RecordPrinters
{
    /// <summary>
    /// Class which prints information to destination stream in table format format, where properties is headers of the table and source data is rows.
    /// </summary>
    /// <seealso cref="IRecordPrinter" />
    public class TablePrinter : IRecordPrinter
    {
        private const int AdditionCharactersToEachProperty = 3;

        private static readonly Type[] LeftPaddingTypes = { typeof(string), typeof(char) };
        private readonly TextWriter writer;
        private (PropertyInfo property, int maxLength, bool isLeftPadding)[] tableData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TablePrinter"/> class.
        /// </summary>
        /// <param name="writer">The destination stream.</param>
        /// <exception cref="ArgumentNullException">Throws when writer is null.</exception>
        public TablePrinter(TextWriter writer) => this.writer =
            writer ?? throw new ArgumentNullException(nameof(writer), "Writer can't be null");

        /// <inheritdoc/>
        public void Print(IEnumerable<FileCabinetRecord> source, IEnumerable<PropertyInfo> selectedFields)
        {
            this.PrintTable(source, selectedFields);
        }

        private void PrintTable(IEnumerable<FileCabinetRecord> source, IEnumerable<PropertyInfo> selectedProperties)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Source can't be null");
            }

            if (selectedProperties is null)
            {
                throw new ArgumentNullException(nameof(selectedProperties), "Selected properties can't be null");
            }

            this.CalculateTableData(source, selectedProperties);
            var summaryTableWidth =
                Math.Max(selectedProperties.Sum(property => property.Name.Length),
                    this.tableData.Sum(data => data.maxLength)) +
                ((AdditionCharactersToEachProperty * this.tableData.Length) + 1);

            this.PrintHeader(summaryTableWidth);
            this.PrintBody(source, summaryTableWidth);
        }

        private void PrintHeader(int tableWidth)
        {
            this.writer.Write('\n');
            this.writer.WriteLine(new string('-', tableWidth));
            this.writer.Write('|');

            for (var index = 0; index < this.tableData.Length; index++)
            {
                this.writer.Write($" {this.tableData[index].property.Name.PadLeft(this.tableData[index].maxLength)} |");
            }

            this.writer.Write('\n');
            this.writer.WriteLine(new string('-', tableWidth));
        }

        private void PrintBody(IEnumerable<FileCabinetRecord> source, int tableWidth)
        {
            foreach (var record in source)
            {
                this.writer.Write('|');
                for (int index = 0; index < this.tableData.Length; index++)
                {
                    var data = $"{this.tableData[index].property.GetValue(record) ?? "null"}";
                    this.writer.Write(this.tableData[index].isLeftPadding
                        ? $" {data.PadLeft(this.tableData[index].maxLength)} |"
                        : $" {data.PadRight(this.tableData[index].maxLength)} |");
                }

                this.writer.Write('\n');
                this.writer.WriteLine(new string('-', tableWidth));
            }
        }

        private void CalculateTableData(IEnumerable<FileCabinetRecord> source, IEnumerable<PropertyInfo> selectedProperties)
        {
            int index = 0;
            this.tableData =
                new (PropertyInfo property, int maxLength, bool isLeftPadding)[selectedProperties is ICollection coll
                    ? coll.Count
                    : selectedProperties.Count()];

            foreach (var property in selectedProperties)
            {
                var maxLength = Math.Max(property.Name.Length,
                    source.Select(record => property.GetValue(record)?.ToString()?.Length ?? "null".Length).Max());
                this.tableData[index++] = (property, maxLength, LeftPaddingTypes.Contains(property.PropertyType));
            }
        }
    }
}
