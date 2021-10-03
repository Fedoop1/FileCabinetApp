using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

#pragma warning disable CA1308 // Normalize strings to uppercase

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "find" command from user input.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        /// <param name="printer">A delegate to a method that print data to the console according to a certain rule.</param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override string Command => "find";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("find", StringComparison.CurrentCultureIgnoreCase))
            {
                var records = this.Find(commandRequest.Parameters);
                this.printer.Invoke(records);
                return;
            }

            if (this.NextHandle != null)
            {
                this.NextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that searches for records by a specific parameter with output to the console.
        /// </summary>
        /// <param name="parameters">Parameter line including 1.search criterion 2.unique information.</param>
        private IEnumerable<FileCabinetRecord> Find(string parameters)
        {
            const int findParam = 0;
            const int findData = 1;
            const int parametersCount = 2;

            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Parameters can't be null or empty");
                return null;
            }

            string[] arrayParameters = parameters.Split(" ", parametersCount);

            if (arrayParameters.Length != parametersCount)
            {
                Console.WriteLine("Invalid parameters count.");
                return null;
            }

            IEnumerable<FileCabinetRecord> records = arrayParameters[findParam].ToLowerInvariant() switch
            {
                "firstname" => this.Service.FindByFirstName(arrayParameters[findData]),
                "lastname" => this.Service.FindByLastName(arrayParameters[findData]),
                "dateofbirth" => this.Service.FindByDayOfBirth(arrayParameters[findData]),
                _ => null,
            };

            return records;
        }
    }
}
