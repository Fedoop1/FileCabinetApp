using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : ServiceCommandHandlerBase
    {
        private Action<bool> exitDelegate;

        public ExitCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        public ExitCommandHandler(IFileCabinetService service, Action<bool> exitDelegate)
            : base(service)
        {
            this.exitDelegate = exitDelegate;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "exit")
            {
                this.Exit();
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
        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.exitDelegate.Invoke(false);
            if (this.service is FileCabinetFilesystemService service)
            {
                service.Dispose();
            }
        }
    }
}
