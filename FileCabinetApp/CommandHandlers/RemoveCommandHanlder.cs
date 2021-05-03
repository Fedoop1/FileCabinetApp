using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHanlder : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest.Command) && commandRequest.Command == "remove")
            {
                Remove(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Removes a record from a data source.
        /// </summary>
        /// <param name="parameters">The identifier of the record to be deleted.</param>
        private static void Remove(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Index is null or empty!");
                return;
            }

            if (!int.TryParse(parameters, out int index))
            {
                throw new ArgumentException("Invalid index.");
            }

            if (Program.FileCabinetService.RemoveRecord(index))
            {
                Console.WriteLine($"Record #{index} is removed.");
                return;
            }

            Console.WriteLine($"Record #{index} doesn't exist.");
        }
    }
}
