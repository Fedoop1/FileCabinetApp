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
            Console.WriteLine("Exiting an application...");
            if (Program.FileCabinetService is FileCabinetFilesystemService service)
            {
                service.Dispose();
            }

            Program.IsRunning = false;
        }

        public override void SetNext(ICommandHandler commandHandler)
        {
            throw new NotImplementedException();
        }
    }
}
