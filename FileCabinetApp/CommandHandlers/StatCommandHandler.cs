using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <inheritdoc/>
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
        /// <param name="parameters">Typically an empty parameter that does not affect method execution.</param>
        private void Stat()
        {
            var recordsCount = this.service.GetStat();
            Console.WriteLine($"{recordsCount.actualRecords} existing record(s). {recordsCount.deletedRecords} deleted record(s).");
        }
    }
}
