using System;
using FileCabinetApp.Interfaces;

#pragma warning disable CA1031 // Do not catch general exception types

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "import" command to import data from file in special format.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const int ImportTypeIndex = 0;
        private const int FilePathIndex = 1;
        private const int ParametersCount = 2;

        private readonly IRecordSnapshotService snapshotService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IFileCabinetService"/> context is necessary for the correct execution of the methods.</param>
        /// <param name="snapshotService">Snapshot service.</param>
        public ImportCommandHandler(IFileCabinetService service, IRecordSnapshotService snapshotService)
            : base(service)
        {
            this.snapshotService = snapshotService;
        }

        /// <inheritdoc/>
        public override string Command => "import";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "import")
            {
                this.Import(commandRequest.Parameters);
                return;
            }

            this.NextHandle?.Handle(commandRequest);
        }

        /// <summary>
        /// Imports data from a external file.
        /// </summary>
        /// <param name="parameters">Includes the data type of the imported file and its path.</param>
        private void Import(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Parameters is null or empty");
                return;
            }

            string[] parametersArray = parameters.Split(" ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parametersArray.Length != ParametersCount)
            {
                Console.WriteLine("Invalid parameters count");
                return;
            }

            try
            {
                var snapshot = this.snapshotService.LoadFrom(parametersArray[ImportTypeIndex], parametersArray[FilePathIndex]);
                var restoreResult = this.Service.Restore(snapshot);
                Console.WriteLine($"{restoreResult} records were imported from {parametersArray[FilePathIndex]}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During saving an error was happened. Error message: {exception.Message}.");
            }
        }
    }
}
