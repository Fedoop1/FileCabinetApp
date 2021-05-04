using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;

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
        private static IFileCabinetService fileCabinetService = new FileCabinetDefaultService();
        private static bool isRunning = true;

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records ?? Array.Empty<FileCabinetRecord>())
            {
                Console.WriteLine(item);
            }
        }

        private static ICommandHandler CreateCommandHandler(IFileCabinetService fileCabinetService)
        {
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(fileCabinetService, UpdateApplicationStatus);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var missedHandler = new MissedCommandHandler();
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHanlder(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);

            createHandler.SetNext(editHandler);
            editHandler.SetNext(exitHandler);
            exitHandler.SetNext(exportHandler);
            exportHandler.SetNext(findHandler);
            findHandler.SetNext(helpHandler);
            helpHandler.SetNext(importHandler);
            importHandler.SetNext(listHandler);
            listHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(removeHandler);
            removeHandler.SetNext(statHandler);
            statHandler.SetNext(missedHandler);

            return createHandler;
        }

        private static void UpdateApplicationStatus(bool status)
        {
            isRunning = status;
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

                        break;
                    case "-v":
                        if (parameterIndex + 1 != parameters.Length && parameters[parameterIndex + 1] == "custom")
                        {
                            fileCabinetService = new FileCabinetCustomService();
                        }
                        else
                        {
                            fileCabinetService = new FileCabinetDefaultService();
                        }

                        break;
                    case string attribute when attribute.Contains("--validation-rule"):
                        attribute = attribute.Substring(attribute.LastIndexOf("=", StringComparison.InvariantCultureIgnoreCase) + 1);
                        if (attribute.ToLower(Culture) == "custom")
                        {
                            fileCabinetService = new FileCabinetCustomService();
                        }
                        else
                        {
                            fileCabinetService = new FileCabinetDefaultService();
                        }

                        break;
                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// The main application method from which the user interacts with all available methods.
        /// </summary>
        /// <param name="args">Command line arguments required to control check parameters.</param>
        private static void Main(string[] args)
        {
            HandlingCommandLineArgs(args);
            var commandHandler = CreateCommandHandler(fileCabinetService);
            Console.WriteLine($"\nFile Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(" ", 2);

                const int commandIndex = 0;
                const int parametersIndex = 1;
                string parameters = inputs.Length > parametersIndex ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest(inputs[commandIndex], parameters));
            }
            while (isRunning);
        }
    }
}