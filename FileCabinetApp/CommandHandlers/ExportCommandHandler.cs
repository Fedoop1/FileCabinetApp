using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "export")
            {
                this.Export(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Make snapshot and export record list in special format to disk. Supports XML and CSV serialization.
        /// </summary>
        /// <param name="parameters">Contain type of export document and filename to save document.</param>
        private void Export(string parameters)
        {
            string[] parameterArray = parameters.Split(" ", 2);

            const int fileTypeIndex = 0;
            const int filePathIndex = 1;
            bool append = true;
            FileCabinetServiceShapshot snapshot = null;

            if (parameters?.Length == 0)
            {
                Console.WriteLine("Empty parameters");
                return;
            }
            else if (parameterArray.Length < 2)
            {
                Console.WriteLine("File path or export type is empty.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(parameterArray[filePathIndex]) || parameterArray[filePathIndex].Length == 0)
            {
                Console.WriteLine("File path is empty or incorrect.");
                return;
            }

            try
            {
                if (File.Exists(parameterArray[filePathIndex]))
                {
                    Console.WriteLine($"File is exist - rewrite {parameterArray[filePathIndex]} ? [Y/N]");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        append = false;
                    }
                }

                using (var streamWriter = new StreamWriter(parameterArray[filePathIndex], append))
                {
                    snapshot = this.service.MakeSnapshot();

                    switch (parameterArray[fileTypeIndex].ToLower(Program.Culture))
                    {
                        case "csv":
                            snapshot.SaveToCSV(streamWriter);
                            break;
                        case "xml":
                            snapshot.SaveToXML(streamWriter);
                            break;
                        default:
                            Console.WriteLine("Unknown export type format.");
                            return;
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"Can't open file {parameterArray[filePathIndex]}");
                return;
            }

            Console.WriteLine($"\nAll records are exported to file {parameterArray[filePathIndex]}.");
        }
    }
}
