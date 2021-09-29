using System;
using System.Collections.Generic;

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
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "find")
            {
                var records = this.Find(commandRequest.Parameters);
                this.printer.Invoke(records);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that searches for records by a specific parameter with output to the console.
        /// </summary>
        /// <param name="parameters">Parameter line including 1.search criterion 2.unique information.</param>
        private IEnumerable<FileCabinetRecord> Find(string parameters)
        {
            const int FindParam = 0;
            const int FindData = 1;
            string[] arrayParameters = parameters.Split(" ", 2);

            IEnumerable<FileCabinetRecord> records = arrayParameters[FindParam] switch
            {
                "firstname" => this.service.FindByFirstName(arrayParameters[FindData]),
                "lastname" => this.service.FindByLastName(arrayParameters[FindData]),
                "dateofbirth" => this.service.FindByDayOfBirth(arrayParameters[FindData]),
                _ => Array.Empty<FileCabinetRecord>()
            };

            return records;
        }
    }
}
