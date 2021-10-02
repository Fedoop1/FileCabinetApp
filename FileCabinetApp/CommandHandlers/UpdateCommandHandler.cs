using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "edit" command from user input.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("update", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Update(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that edits information about a specific record.
        /// </summary>
        /// <param name="parameters">A parameter consisting of a unique identifier required to search for a record.</param>
        private void Update(string parameters)
        {
            try
            {
                this.Service.EditRecord(null);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During updating an error was happened. Error message: {exception.Message}.");
                throw;
            }
        }
    }
}
