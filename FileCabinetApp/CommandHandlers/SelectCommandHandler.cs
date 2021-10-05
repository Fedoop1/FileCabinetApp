using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.CommandHandlers.CommandHandlerExtensions;

#pragma warning disable CA1308 // Normalize strings to uppercase

namespace FileCabinetApp.CommandHandlers
{
    public class SelectCommandHandler
        : ServiceCommandHandlerBase
    {
        private const int ParametersIndex = 0;
        private const int PredicateIndex = 1;
        private const int SelectWithWhere = 2;
        private const char SelectAllColumns = '*';
        private static readonly int FileCabinetRecordFieldsCount = typeof(FileCabinetRecord).GetProperties().Length;

        private readonly IRecordPrinter printer;

        public override string Command => "select";

        public SelectCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer), "Printer can't be null");
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null && commandRequest.Command.Contains("select", StringComparison.CurrentCultureIgnoreCase))
            {
                var result = this.SelectRecords(commandRequest.Parameters);

                if (result.selectedFields is ICollection collection &&
                    collection.Count == FileCabinetRecordFieldsCount)
                {
                    this.printer.Print(result.source);
                }
                else
                {
                    this.printer.Print(result.source, result.selectedFields);
                }

                return;
            }

            this.NextHandle?.Handle(commandRequest);
        }

        private static IEnumerable<PropertyInfo> ExtractProperties(IEnumerable<string> source)
        {
            var result = new List<PropertyInfo>();

            foreach (var column in source)
            {
                result.Add(typeof(FileCabinetRecord).GetProperties().First(property =>
                    property.Name.Contains(column, StringComparison.CurrentCultureIgnoreCase)));
            }

            return result;
        }

        private (IEnumerable<FileCabinetRecord> source, IEnumerable<PropertyInfo> selectedFields) SelectRecords(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Parameters can't be null or empty");
                return (this.Service.GetRecords(), typeof(FileCabinetRecord).GetProperties());
            }

            var parametersArray = parameters.ToLowerInvariant().Split("where",
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            string columnsString = parameters[..parametersArray[ParametersIndex].Length];
            string whereString = parametersArray.Length == SelectWithWhere ? parameters[^parametersArray[PredicateIndex].Length..] : null;

            var arrayOfColumns =
                columnsString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var whereKeyValuePair = whereString is not null
                ? ExtractKeyValuePair(whereString.ToLowerInvariant(), new[] { "and" })
                : null;

            var predicate = GeneratePredicate(whereKeyValuePair);

            var propertiesToSelect = arrayOfColumns[0].StartsWith(SelectAllColumns)
                ? typeof(FileCabinetRecord).GetProperties()
                : ExtractProperties(arrayOfColumns);

            return (this.Service.GetRecords().Where(rec => predicate(rec)), propertiesToSelect);
        }
    }
}
