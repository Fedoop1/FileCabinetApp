using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        ///<inheritdoc/>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
        }

        ///<inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "list")
            {
                var records = this.List();
                this.printer.Invoke(records);
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
        private IEnumerable<FileCabinetRecord> List()
        {
            var recordsArray = this.service.GetRecords();

            if (recordsArray.Count == 0)
            {
                Console.WriteLine("Records list is empty.");
            }

            return recordsArray;
        }
    }
}
