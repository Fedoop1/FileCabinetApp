using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class CreateCommandHandler : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "create")
            {
                Create(commandRequest.Parameters);
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
        /// <param name="parameters">The parameter does not affect the execution of the method.</param>
        private static void Create(string parameters)
        {
            Program.RecordDataContainer.InputData();
            int result = Program.FileCabinetService.CreateRecord(Program.RecordDataContainer);

            Console.WriteLine($"Record #{result} is created.");
        }
    }
}
