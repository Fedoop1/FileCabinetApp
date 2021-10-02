using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "remove" command to import data from file in special format.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IFileCabinetService"/> context is necessary for the correct execution of the methods.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Delete(commandRequest.Parameters);
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
        private void Delete(string parameters)
        {
            try
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

                this.Service.RemoveRecord(null);
                Console.WriteLine($"Record #{index} was removed.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During deleting an error was happened. Error message: {exception.Message}.");
            }
        }
    }
}
