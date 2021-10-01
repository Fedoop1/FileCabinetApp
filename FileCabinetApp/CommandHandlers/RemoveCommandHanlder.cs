using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "remove" command to import data from file in special format.
    /// </summary>
    public class RemoveCommandHanlder : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHanlder"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IFileCabinetService"/> context is necessary for the correct execution of the methods.</param>
        public RemoveCommandHanlder(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "remove")
            {
                this.Remove(commandRequest.Parameters);
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
        private void Remove(string parameters)
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

            if (this.Service.RemoveRecord(index))
            {
                Console.WriteLine($"Record #{index} is removed.");
                return;
            }

            Console.WriteLine($"Record #{index} doesn't exist.");
        }
    }
}
