using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

#pragma warning disable CA1308 // Normalize strings to uppercase
#pragma warning disable CA1031 // Do not catch general exception types

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "Insert" command from user input.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const int FieldsIndex = 0;
        private const int ValuesIndex = 1;
        private const int ParametersCount = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override string Command => "insert";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("insert", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Insert(commandRequest.Parameters);
                return;
            }

            if (this.NextHandle != null)
            {
                this.NextHandle.Handle(commandRequest);
            }
        }

        private static Dictionary<string, string> InitializeDictionary(string[] keys, string[] values)
        {
            var result = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            for (int index = 0; index < keys.Length; index++)
            {
                result.Add(keys[index], values[index]);
            }

            return result;
        }

        private static FileCabinetRecord InitializeRecord(Dictionary<string, string> keyValueTuple)
        {
            var result = new FileCabinetRecord();

            foreach (var property in result.GetType().GetProperties())
            {
                var parseMethod = property.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .FirstOrDefault(method =>
                        method.Name.Contains("Parse") && method.GetParameters().Length == 1);
                property!.SetValue(result, property.PropertyType.Name == "String" ? keyValueTuple[property.Name] : parseMethod!.Invoke(null, new object[] { keyValueTuple[property.Name] }));
            }

            return result;
        }

        private static string[] ParseValueTuple(string values) => values.Trim(' ', '(', ')').Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.CurrentCultureIgnoreCase).Select(value => value.Trim('\u0027')).ToArray();

        /// <summary>
        /// Create a new <see cref="FileCabinetRecord"/>.
        /// </summary>
        private void Insert(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    Console.WriteLine("Parameters can't be null or empty");
                    return;
                }

                var parametersArray = parameters.ToLowerInvariant().Split(new[] { "values" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parametersArray.Length != ParametersCount)
                {
                    Console.WriteLine("Invalid parameters count");
                    return;
                }

                var parametersString = parameters[..parametersArray[FieldsIndex].Length];
                var valuesString = parameters[^parametersArray[ValuesIndex].Length..];

                var parameterArray = ParseValueTuple(parametersString);
                var valueArray = ParseValueTuple(valuesString);

                if (parameterArray.Length != valueArray.Length)
                {
                    Console.WriteLine("Parameters count doesn't equals values count");
                    return;
                }

                var recordKeyValue = InitializeDictionary(parameterArray, valueArray);
                var record = InitializeRecord(recordKeyValue);
                this.Service.AddRecord(record);

                Console.WriteLine($"Record #{record.Id} is created.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During inserting an error was happened. Error message: {exception.Message}.");
            }
        }
    }
}
