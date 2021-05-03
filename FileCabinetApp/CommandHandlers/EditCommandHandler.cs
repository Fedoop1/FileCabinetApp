using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "edit")
            {
                Edit(commandRequest.Parameters);
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
        private static void Edit(string parameters)
        {
            Program.RecordDataContainer.InputData();

            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"Id is incorrect.");
                return;
            }

            Program.FileCabinetService.EditRecord(id, Program.RecordDataContainer);
        }
    }
}
