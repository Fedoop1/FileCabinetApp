using System;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "import" command to import data from file in special format.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const int ImportTypeIndex = 0;
        private const int FilePathIndex = 1;

        private readonly IServiceProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IFileCabinetService"/> context is necessary for the correct execution of the methods.</param>
        public ImportCommandHandler(IFileCabinetService service, IServiceProvider provider)
            : base(service)
        {
            this.provider = provider;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "import")
            {
                this.Import(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Imports data from a external file.
        /// </summary>
        /// <param name="parameters">Includes the data type of the imported file and its path.</param>
        private void Import(string parameters)
        {
            var parametersArray = new string[2];

            if (string.IsNullOrEmpty(parameters) || (parametersArray = parameters.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).Length != 2)
            {
                Console.WriteLine("Invalid parameters");
                return;
            }

            try
            {
                var snapshotService = this.provider.GetService<IRecordSnapshotService>();
                snapshotService.LoadFrom(parametersArray[ImportTypeIndex], parametersArray[FilePathIndex]);
                var restoreResult = this.Service.Restore(new RecordShapshot(snapshotService.Records));
                Console.WriteLine($"{restoreResult} records were imported from {parametersArray[FilePathIndex]}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During saving an error was happened. Error message: {exception.Message}.");
            }
        }
    }
}
