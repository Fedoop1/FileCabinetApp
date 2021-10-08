using System;
using System.IO;
using FileCabinetApp.Interfaces;

#pragma warning disable CA1031 // Do not catch general exception types

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "export" command from user input.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const int FileTypeIndex = 0;
        private const int FilePathIndex = 1;
        private const int ParametersCount = 2;

        private readonly IRecordSnapshotService snapshotService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        /// <param name="snapshot">Snapshot service.</param>
        public ExportCommandHandler(IFileCabinetService service, IRecordSnapshotService snapshot)
            : base(service)
        {
            this.snapshotService = snapshot;
        }

        /// <inheritdoc/>
        public override string Command => "export";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "export")
            {
                this.Export(commandRequest.Parameters);
                return;
            }

            if (this.NextHandle != null)
            {
                this.NextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Make snapshot and export record list in special format to disk. Supports XML and CSV serialization.
        /// </summary>
        /// <param name="parameters">Contain type of export document and filename to save document.</param>
        private void Export(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Parameters is null or empty");
                return;
            }

            string[] parametersArray = parameters.Split(" ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parametersArray.Length != ParametersCount)
            {
                Console.WriteLine("Invalid parameters count.");
            }

            try
            {
                bool append = true;
                if (File.Exists(parametersArray[FilePathIndex]))
                {
                    Console.WriteLine($"File is exist - rewrite {parametersArray[FilePathIndex]} ? [Y/N]");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        append = false;
                    }
                }

                this.snapshotService.SaveTo(parametersArray[FileTypeIndex], parametersArray[FilePathIndex], append);
                Console.WriteLine($"\nAll records are exported to file {parametersArray[FilePathIndex]}.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During saving an error was happened. Error message: {exception.Message}.");
            }
        }
    }
}
