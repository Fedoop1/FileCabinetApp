using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        /// <inheritdoc/>
        public FindCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "find")
            {
                this.Find(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that searches for records by a specific parameter with output to the console.
        /// </summary>
        /// <param name="parameters">Parameter line including 1.search criterion 2.unique information.</param>
        private void Find(string parameters)
        {
            const int FindParam = 0;
            const int FindData = 1;

            FileCabinetRecord[] records = null;
            string[] arrayParameters = parameters.Split(" ", 2);

            records = arrayParameters[FindParam] switch
            {
                "firstname" => this.service.FindByFirstName(arrayParameters[FindData]),
                "lastname" => this.service.FindByLastName(arrayParameters[FindData]),
                "dateofbirth" => this.service.FindByDayOfBirth(arrayParameters[FindData]),
                _ => Array.Empty<FileCabinetRecord>()
            };

            if (records.Length == 0)
            {
                Console.WriteLine("There are no records with this parameters.");
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine(record);
            }

            return;
        }
    }
}
