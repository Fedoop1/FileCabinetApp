using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class from where all available functions are called.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Fields containing the culture of the user, which is necessary for the correct operation of the application.
        /// </summary>
        public static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
        private const string DeveloperName = "Nikita Malukov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static IFileCabinetService fileCabinetService = new FileCabinetDefaultService();
        private static FileCabinetRecordData recordDataContainer = new FileCabinetRecordData("default");
        private static bool isRunning = true;

        /// <summary>
        /// A tuple that includes the name of the command and the Action delegate to call the function.
        /// </summary>
        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        /// <summary>
        /// The jagged array needed to get help with all the available commands consists of the command name, a short description, and an extended description.
        /// </summary>
        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the count if records", "The 'stat' command prints the count of the records." },
            new string[] { "create", "create the record in file cabinet", "The 'create' command create the record in file cabinet." },
            new string[] { "list", "prints the list if records", "The 'list' command prints the list of the records." },
            new string[] { "edit", "edits the record", "The 'edit' command edits the value of the record." },
            new string[] { "find", "finds a record", "The 'find' command find a record by the specified parameter. Example '>find [param] [data]." },
            new string[] { "export", "Make snapshot and save it to file.", "The export command make snapshot of you record list and save it to special file." },
            new string[] { "import", "Import records from external storage.", "The import command imports records from a file in two possible formats XML and CSV." },
            new string[] { "remove", "Remove selected record.", "The command remove record at the selected index." },
            new string[] { "purge", "Defragment the db file.", "The command invokes an algorithm that destroys deleted records from the file." },
        };

        private static void Purge(string parameters)
        {
            string result = fileCabinetService.Purge();
            Console.WriteLine(result);
        }

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

            if (fileCabinetService.RemoveRecord(index))
            {
                Console.WriteLine($"Record #{index} is removed.");
                return;
            }

            Console.WriteLine($"Record #{index} doesn't exist.");
        }

        /// <summary>
        /// Make snapshot and export record list in special format to disk. Supports XML and CSV serialization.
        /// </summary>
        /// <param name="parameters">Contain type of export document and filename to save document.</param>
        private static void Export(string parameters)
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
                    snapshot = fileCabinetService.MakeSnapshot();

                    switch (parameterArray[fileTypeIndex].ToLower(Culture))
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
                    FileCabinetServiceShapshot snapshot = fileCabinetService.MakeSnapshot();
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
                    fileCabinetService.Restore(snapshot);
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"Can't open file {parametersArray[FilePathIndex]}");
                return;
            }
        }

        /// <summary>
        /// A method that accepts command line parameters to control the check rules depending on the passed argument.
        /// </summary>
        /// <param name="parameters">An array of arguments passed to the Main method.</param>
        private static void HandlingCommandLineArgs(string[] parameters)
        {
            Console.Write("$FileCabinetApp.exe");

            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                Console.Write(" " + parameters[parameterIndex]);
                switch (parameters[parameterIndex])
                {
                    case "--storage":
                    case "-s":
                        if (parameterIndex + 1 != parameters.Length && parameters[parameterIndex + 1].ToLower(Culture) == "memory")
                        {
                            fileCabinetService = new FileCabinetDefaultService();
                        }
                        else
                        {
                            fileCabinetService = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite));
                        }

                        recordDataContainer = new FileCabinetRecordData("default");
                        break;
                    case "-v":
                        if (parameterIndex + 1 != parameters.Length && parameters[parameterIndex + 1] == "custom")
                        {
                            fileCabinetService = new FileCabinetCustomService();
                            recordDataContainer = new FileCabinetRecordData("custom");
                        }
                        else
                        {
                            fileCabinetService = new FileCabinetDefaultService();
                            recordDataContainer = new FileCabinetRecordData("default");
                        }

                        break;
                    case string attribute when attribute.Contains("--validation-rule"):
                        attribute = attribute.Substring(attribute.LastIndexOf("=", StringComparison.InvariantCultureIgnoreCase) + 1);
                        if (attribute.ToLower(Culture) == "custom")
                        {
                            fileCabinetService = new FileCabinetCustomService();
                            recordDataContainer = new FileCabinetRecordData("custom");
                        }
                        else
                        {
                            fileCabinetService = new FileCabinetDefaultService();
                            recordDataContainer = new FileCabinetRecordData("default");
                        }

                        break;
                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// A method that searches for records by a specific parameter with output to the console.
        /// </summary>
        /// <param name="parameters">Parameter line including 1.search criterion 2.unique information.</param>
        private static void Find(string parameters)
        {
            const int FindParam = 0;
            const int FindData = 1;

            FileCabinetRecord[] records = null;
            string[] arrayParameters = parameters.Split(" ", 2);

            records = arrayParameters[FindParam] switch
            {
                "firstname" => fileCabinetService.FindByFirstName(arrayParameters[FindData]),
                "lastname" => fileCabinetService.FindByLastName(arrayParameters[FindData]),
                "dateofbirth" =>fileCabinetService.FindByDayOfBirth(arrayParameters[FindData]),
                _ => Array.Empty<FileCabinetRecord>()
            };

            if (records.Length == 0)
            {
                Console.WriteLine("There are no records with this parameters.");
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }

        private static void Create(string parameters)
        {
            recordDataContainer.InputData();
            int result = fileCabinetService.CreateRecord(recordDataContainer);

            Console.WriteLine($"Record #{result} is created.");
        }

        /// <summary>
        /// A method that edits information about a specific record.
        /// </summary>
        /// <param name="parameters">A parameter consisting of a unique identifier required to search for a record.</param>
        private static void Edit(string parameters)
        {
            recordDataContainer.InputData();

            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"Id is incorrect.");
                return;
            }

            fileCabinetService.EditRecord(id, recordDataContainer);
        }

        /// <summary>
        /// A method that displays the number of records in the application.
        /// </summary>
        /// <param name="parameters">Typically an empty parameter that does not affect method execution.</param>
        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        /// <summary>
        /// A method that returns all available records in the application, outputting from the console.
        /// </summary>
        /// <param name="parameters">Typically an empty parameter that does not affect method execution.</param>
        private static void List(string parameters)
        {
            var recordsArray = fileCabinetService.GetRecords();

            foreach (var record in recordsArray)
            {
                Console.WriteLine(record);
            }
        }

        /// <summary>
        /// The main application method from which the user interacts with all available methods.
        /// </summary>
        /// <param name="args">Command line arguments required to control check parameters.</param>
        private static void Main(string[] args)
        {
            HandlingCommandLineArgs(args);
            Console.WriteLine($"\nFile Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(" ", 2);

                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        /// <summary>
        /// A method that is executed if a non-existent command is selected.
        /// </summary>
        /// <param name="command">User invoked command.</param>
        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        /// <summary>
        /// Method displays all auxiliary information about existing methods, or additional information about a specific method.
        /// </summary>
        /// <param name="parameters">Parameter with the choice of a specific command to display help about it.</param>
        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// A method that produces a safe exit from the application.
        /// </summary>
        /// <param name="parameters">Typically an empty parameter that does not affect method execution.</param>
        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            if (fileCabinetService is FileCabinetFilesystemService service)
            {
                service.Dispose();
            }

            isRunning = false;
        }
    }
}