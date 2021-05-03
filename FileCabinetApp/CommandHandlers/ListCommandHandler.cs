using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        ///<inheritdoc/>
        public ListCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        ///<inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "list")
            {
                this.List();
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that returns all available records in the application, outputting from the console.
        /// </summary>
        private void List()
        {
            var recordsArray = this.service.GetRecords();

            foreach (var record in recordsArray)
            {
                Console.WriteLine(record);
            }
        }
    }
}
