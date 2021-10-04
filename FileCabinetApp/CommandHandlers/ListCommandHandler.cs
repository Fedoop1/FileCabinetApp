using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "list" command from user input.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        /// <param name="printer">A delegate to a method that print data to the console according to a certain rule.</param>
        public ListCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer), "Printer can't be null");
        }

        /// <inheritdoc/>
        public override string Command => "list";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "list")
            {
                this.printer.Print(this.List());
                return;
            }

            if (this.NextHandle != null)
            {
                this.NextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that returns all available records in the application, outputting from the console.
        /// </summary>
        private IEnumerable<FileCabinetRecord> List() => this.Service.GetRecords();
    }
}
