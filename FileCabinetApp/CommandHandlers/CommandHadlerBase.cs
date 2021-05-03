using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class CommandHadlerBase : ICommandHandler
    {
        protected ICommandHandler nextHandle;

        public abstract void Handle(AppCommandRequest commandRequest);

        public abstract void SetNext(ICommandHandler commandHandler);
    }
}
