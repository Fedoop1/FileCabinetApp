using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "import" command to import data from file in special format.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IFileCabinetService"/> context is necessary for the correct execution of the methods.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "import")
            {
                this.Import(commandRequest.Parameters);
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
            const int ImportTypeIndex = 0;
            const int FilePathIndex = 1;
            var parametersArray = parameters.Split(" ", 2);
            if (parametersArray.Length < 2)
            {
                Console.WriteLine("Import parameters are incorrect.");
                return;
            }

            try
            {
                using var fileStream = new FileStream(parametersArray[FilePathIndex], FileMode.Open, FileAccess.Read);
                FileCabinetServiceShapshot snapshot = this.service.MakeSnapshot();
                switch (parametersArray[ImportTypeIndex].ToUpperInvariant())
                {
                    case "CSV":
                        snapshot.LoadFromCSV(new StreamReader(fileStream));
                        break;
                    case "XML":
                        snapshot.LoadFromXML(fileStream);
                        break;
                    default:
                        Console.WriteLine("Unknown import format.");
                        break;
                }

                Console.WriteLine($"{snapshot.Records.Count} records were imported from {parametersArray[FilePathIndex]}");
                this.service.Restore(snapshot);
                return;
            }
            catch (IOException)
            {
                Console.WriteLine($"Can't open file {parametersArray[FilePathIndex]}");
                return;
            }
        }
    }
}
