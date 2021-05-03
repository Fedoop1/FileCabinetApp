using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "stat")
            {
                Stat();
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
        private static void Stat()
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.actualRecords} existing record(s). {recordsCount.deletedRecords} deleted record(s).");
        }
    }
}
