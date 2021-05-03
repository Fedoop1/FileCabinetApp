using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "create")
            {
                this.Create(commandRequest.Parameters);
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
        private void Create(string parameters)
        {
            int result = this.service.CreateRecord();

            Console.WriteLine($"Record #{result} is created.");
        }
    }
}
