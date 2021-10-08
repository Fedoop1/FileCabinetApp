using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.CommandHandlers.CommandHandlerExtensions;

#pragma warning disable CA1308 // Normalize strings to uppercase
#pragma warning disable CA1031 // Do not catch general exception types

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "Update" command from user input.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const int SetItemsIndex = 0;
        private const int WhereItemsIndex = 1;

        private const int UpdateWithoutPredicate = 1;
        private const int UpdateWithPredicate = 2;

        private const int SetKeyWordOffset = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override string Command => "update";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("update", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Update(commandRequest.Parameters);
                return;
            }

            this.NextHandle?.Handle(commandRequest);
        }

        private static void UpdateRecord(FileCabinetRecord record, IDictionary<PropertyInfo, object> source)
        {
            foreach (KeyValuePair<PropertyInfo, object> propertyValuePair in source)
            {
                propertyValuePair.Key.SetValue(record, propertyValuePair.Value);
            }
        }

        private static Dictionary<PropertyInfo, object> BindPropertyAndValue(IDictionary<string, string> source)
        {
            Dictionary<PropertyInfo, object> result = new ();

            foreach (var keyValuePair in source)
            {
                var property = typeof(FileCabinetRecord).GetProperties().FirstOrDefault(property => property.Name.Contains(keyValuePair.Key, StringComparison.CurrentCultureIgnoreCase));
                var parseMethod = property?.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .FirstOrDefault(method => method.Name.Contains("Parse") && method.GetParameters().Length == 1);
                var value = property?.PropertyType.Name == "String" ? keyValuePair.Value : parseMethod?.Invoke(null, new object[] { keyValuePair.Value });

                if (!result.TryAdd(property, value))
                {
                    Console.WriteLine("An exception happened during 'SET' initialization");
                }
            }

            return result;
        }

        /// <summary>
        /// A method that edits information about a specific record.
        /// </summary>
        /// <param name="parameters">A parameter consisting of a unique identifier required to search for a record.</param>
        private void Update(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    Console.WriteLine("Parameters can't be null or empty");
                    return;
                }

                var parametersArray = parameters.ToLowerInvariant().Split(new[] { "set", "where" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parametersArray.Length < UpdateWithoutPredicate)
                {
                    Console.WriteLine("Invalid parameters count");
                    return;
                }

                string setString = parameters.Substring(SetKeyWordOffset, parametersArray[SetItemsIndex].Length);
                string whereString = parametersArray.Length is UpdateWithPredicate ? parameters[^parametersArray[WhereItemsIndex].Length..] : null;

                var whereKeyValuePair = whereString is not null ? ExtractKeyValuePair(whereString.ToLowerInvariant(), new[] { "and" }) : null;
                var setKeyValuePair = ExtractKeyValuePair(setString, new[] { "," });
                var propertyKeyValuePair = BindPropertyAndValue(setKeyValuePair);

                var predicate = GeneratePredicate(whereKeyValuePair);

                List<int> updatedRecords = new ();
                foreach (var record in this.Service.GetRecords(new RecordQuery(predicate, parameters)))
                {
                    UpdateRecord(record, propertyKeyValuePair);
                    this.Service.EditRecord(record);
                    updatedRecords.Add(record.Id);
                }

                Console.WriteLine(updatedRecords.Count > 0
                    ? $"Record(s) {string.Join(", ", updatedRecords.Select(record => $"#{record}")).TrimEnd(',', ' ')} were updated."
                    : "There isn't record to update");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During updating an error was happened. Error message: {exception.Message}.");
            }
        }
    }
}
