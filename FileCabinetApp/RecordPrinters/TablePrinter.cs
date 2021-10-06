using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.RecordPrinters
{
    public class TablePrinter : IRecordPrinter
    {
        private const int AdditionCharactersToEachProperty = 3;

        private readonly TextWriter writer;
        private static readonly Type[] leftPaddingTypes = { typeof(string), typeof(char) };
        private (PropertyInfo property, int maxLength, bool isLeftPadding)[] tableData;

        public TablePrinter(TextWriter writer) => this.writer =
            writer ?? throw new ArgumentNullException(nameof(writer), "Writer can't be null");

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
            int summaryTableWidth =
                Math.Max(selectedProperties.Sum(property => property.Name.Length),
                    this.tableData.Sum(data => data.maxLength)) +
                ((AdditionCharactersToEachProperty * this.tableData.Length) + 1);

            this.PrintHeader(this.tableData, summaryTableWidth);
            this.PrintBody(this.tableData, source, summaryTableWidth);
        }

        private void PrintHeader((PropertyInfo propertyInfo, int maxLength, bool isLeftPadding)[] tableData, int tableWidth)
        {
            this.writer.Write('\n');
            this.writer.WriteLine(new string('-', tableWidth));
            this.writer.Write('|');

            for (var index = 0; index < tableData.Length; index++)
            {
                this.writer.Write($" {tableData[index].propertyInfo.Name.PadLeft(tableData[index].maxLength)} |");
            }

            this.writer.Write('\n');
            this.writer.WriteLine(new string('-', tableWidth));
        }

        private void PrintBody((PropertyInfo propertyInfo, int maxLength, bool isLeftPadding)[] tableData, IEnumerable<FileCabinetRecord> source, int tableWidth)
        {
            foreach (var record in source)
            {
                this.writer.Write('|');
                for (int index = 0; index < this.tableData.Length; index++)
                {
                    var data = $"{tableData[index].propertyInfo.GetValue(record) ?? "null"}";
                    this.writer.Write(tableData[index].isLeftPadding
                        ? $" {data.PadLeft(tableData[index].maxLength)} |"
                        : $" {data.PadRight(tableData[index].maxLength)} |");
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
                this.tableData[index++] = (property, maxLength, leftPaddingTypes.Contains(property.PropertyType));
            }
        }
    }
}
