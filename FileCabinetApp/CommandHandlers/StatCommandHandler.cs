using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "stat" command from user input.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "stat")
            {
                this.Stat();
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that displays the number of records in the application.
        /// </summary>
        private void Stat()
        {
            var (actualRecords, deletedRecords) = this.Service.GetStat();
            Console.WriteLine($"{actualRecords} existing record(s). {deletedRecords} deleted record(s).");
        }

        public override string Command => "stat";
    }
}
