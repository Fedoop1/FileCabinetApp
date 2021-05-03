using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class HelpCommandHandler : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command != "help")
            {
                if (this.nextHandle != null)
                {
                    this.nextHandle.Handle(commandRequest);
                    return;
                }

                new MissedCommandHandler().Handle(null);
                return;
            }
        }

        public override void SetNext(ICommandHandler commandHandler)
        {
            throw new NotImplementedException();
        }
    }
}
