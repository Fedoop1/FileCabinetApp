using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ImportCommandHandler : CommandHadlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "import")
            {
                Import(commandRequest.Parameters);
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
        private static void Import(string parameters)
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
                using (var fileStream = new FileStream(parametersArray[FilePathIndex], FileMode.Open, FileAccess.Read))
                {
                    FileCabinetServiceShapshot snapshot = Program.FileCabinetService.MakeSnapshot();
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
                    Program.FileCabinetService.Restore(snapshot);
                    return;
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"Can't open file {parametersArray[FilePathIndex]}");
                return;
            }
        }
    }
}
