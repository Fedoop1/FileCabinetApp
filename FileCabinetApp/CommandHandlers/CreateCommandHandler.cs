using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "create" command from user input.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const int FieldsIndex = 0;
        private const int ValuesIndex = 1;
        private const int ParametersCount = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "insert")
            {
                this.Create(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Create a new <see cref="FileCabinetRecord"/>.
        /// </summary>
        private void Create(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            var parametersArray = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parametersArray.Length != ParametersCount)
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            var parameterArray = ParseValueTuple(parametersArray[FieldsIndex]);
            var valueArray = ParseValueTuple(parametersArray[ValuesIndex]);

            if (parametersArray.Length != valueArray.Length)
            {
                Console.WriteLine("Parameters count doesn't equals values count");
                return;
            }

            var recordKeyValue = InitializeDictionary(parameterArray, valueArray);
            var result = this.Service.AddRecord(InitializeRecord(recordKeyValue));

            Console.WriteLine($"Record #{result} is created.");
        }

        private static Dictionary<string, string> InitializeDictionary(string[] keys, string[] values)
        {
            var result = new Dictionary<string, string>();
            for (int index = 0; index < keys.Length; index++)
            {
                result.Add(keys[index], values[index]);
            }

            return result;
        }

        private static FileCabinetRecord InitializeRecord(Dictionary<string, string> keyValueTuple)
        {
            var result = new FileCabinetRecord();

            foreach (var parameter in keyValueTuple.Keys)
            {
                result.GetType().GetProperty(parameter, BindingFlags.Public | BindingFlags.IgnoreCase)?.SetValue(result, keyValueTuple[parameter]);
            }

            return result;
        }

        private static string[] ParseValueTuple(string values) => values.Trim(' ', '(', ')').Split(',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray();
    }
}
