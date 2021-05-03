using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "exit")
            {
                Exit();
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that produces a safe exit from the application.
        /// </summary>
        private static void Exit()
        {
            Console.WriteLine("Exiting an application...");
            if (Program.FileCabinetService is FileCabinetFilesystemService service)
            {
                service.Dispose();
            }

            Program.IsRunning = false;
        }
    }
}
