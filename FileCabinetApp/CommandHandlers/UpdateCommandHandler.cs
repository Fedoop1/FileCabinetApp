using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "Update" command from user input.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const int SetItemsIndex = 0;
        private const int WhereItemsIndex = 1;

        private const int KeyIndex = 0;
        private const int ValuesIndex = 1;

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
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("update", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Update(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
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

                var parametersArray = parameters.ToLowerInvariant().Split(new[] { "set", "where" },
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parametersArray.Length < UpdateWithoutPredicate)
                {
                    Console.WriteLine("Invalid parameters count");
                    return;
                }

                string setString = parameters.Substring(SetKeyWordOffset, parametersArray[SetItemsIndex].Length);
                string whereString = parametersArray.Length is UpdateWithPredicate ? parameters[^parametersArray[WhereItemsIndex].Length..] : null;

                Dictionary<string, string> whereKeyValuePair = whereString is not null ? ExtractKeyValuePair(whereString.ToLowerInvariant(), new[] { "and" }) : null;
                Dictionary<string, string> setKeyValuePair = ExtractKeyValuePair(setString, new[] { "," });
                Dictionary<PropertyInfo, object> propertyKeyValuePair = BindPropertyAndValue(setKeyValuePair);

                var predicate = GeneratePredicate(whereKeyValuePair);

                List<int> updatedRecords = new ();
                foreach (var record in this.Service.GetRecords().Where(record => predicate(record)))
                {
                    UpdateRecord(record, propertyKeyValuePair);
                    this.Service.EditRecord(record);
                    updatedRecords.Add(record.Id);
                }

                Console.WriteLine(updatedRecords.Count > 0
                    ? $"Record(s) {string.Join(", ", updatedRecords.Select(record => $"#{record}")).TrimEnd(new[] { ',', ' ' })} were updated."
                    : "There isn't record to update");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During updating an error was happened. Error message: {exception.Message}.");
            }
        }

        private static Predicate<FileCabinetRecord> GeneratePredicate(Dictionary<string, string> keyValuePair)
        {
            if (keyValuePair is null)
            {
                return record => true;
            }

            Predicate<FileCabinetRecord> intermediateResult = null;

            foreach (var pair in keyValuePair)
            {
                var property =
                    typeof(FileCabinetRecord).GetProperties().FirstOrDefault(property => property.Name.Contains(pair.Key, StringComparison.CurrentCultureIgnoreCase));
                intermediateResult += record => property.GetValue(record).ToString() == pair.Value;
            }

            return intermediateResult!.GetInvocationList().Length > 0 ? CombinePredicateIntoOneMethod(intermediateResult) : intermediateResult;

            static Predicate<FileCabinetRecord> CombinePredicateIntoOneMethod(Predicate<FileCabinetRecord> predicate)
            {
                return record =>
                {
                    foreach (var method in predicate.GetInvocationList())
                    {
                        if (((Predicate<FileCabinetRecord>)method).Invoke(record) is false)
                        {
                            return false;
                        }
                    }

                    return true;
                };
            }
        }

        private static Dictionary<PropertyInfo, object> BindPropertyAndValue(Dictionary<string, string> source)
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

        private static void UpdateRecord(FileCabinetRecord record, Dictionary<PropertyInfo, object> source)
        {
            foreach (KeyValuePair<PropertyInfo, object> propertyValuePair in source)
            {
                propertyValuePair.Key.SetValue(record, propertyValuePair.Value);
            }
        }

        private static Dictionary<string, string> ExtractKeyValuePair(string source, string[] separator)
        {
            var result = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            var parameterPairs = source.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var parameterValuePair = parameterPairs.Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            foreach (var pair in parameterValuePair)
            {
                result.Add(pair[KeyIndex], pair[ValuesIndex].Trim('\u0027'));
            }

            return result;
        }

        public override string Command => "update";
    }
}
