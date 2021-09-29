using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Decorators;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class from where all available functions are called.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Nikita Malukov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static IConfiguration configuration;

        private static bool isRunning = true;
        private static IFileCabinetService service;
        private static IValidationSettings settings;

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records ?? Array.Empty<FileCabinetRecord>())
            {
                Console.WriteLine(item);
            }
        }

        private static ICommandHandler CreateCommandHandler()
        {
            var createHandler = new CreateCommandHandler(service);
            var editHandler = new EditCommandHandler(service);
            var exitHandler = new ExitCommandHandler(service, UpdateApplicationStatus);
            var exportHandler = new ExportCommandHandler(service);
            var findHandler = new FindCommandHandler(service, DefaultRecordPrint);
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(service);
            var listHandler = new ListCommandHandler(service, DefaultRecordPrint);
            var missedHandler = new MissedCommandHandler();
            var purgeHandler = new PurgeCommandHandler(service);
            var removeHandler = new RemoveCommandHanlder(service);
            var statHandler = new StatCommandHandler(service);

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
        private static (IFileCabinetService service, IValidationSettings settings) Configure(string[] parameters)
        {
            Console.Write("$FileCabinetApp.exe");

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("validation-rules.json")
                .AddCommandLine(parameters, new Dictionary<string, string>
                {
                    ["--validation-rules"] = "validation-rules",
                    ["-v"] = "v",
                    ["-s"] = "s",
                    ["--storage"] = "storage",
                })
                .Build();

            IValidationSettings settings = configuration.GetSection("Default").Get<ValidationSettings>();
            IFileCabinetService service = new FileCabinetMemoryService(settings);

            foreach (var parameter in parameters)
            {
                Console.WriteLine(parameter);
            }

            if (configuration["validation-rules"] == "custom" || configuration["v"] == "custom")
            {
                settings = configuration.GetSection("Custom").Get<ValidationSettings>();
                service = new FileCabinetMemoryService(settings);
            }

            if (configuration["storage"] == "file" || configuration["s"] == "file")
            {
                service = new FileCabinetFilesystemService("cabinet-records.db", settings);
            }

            if (parameters.Contains("use-stopwatch"))
            {
                service = new ServiceMeter(service);
            }

            if (parameters.Contains("use-logger"))
            {
                service = new ServiceLogger(service, new Logger<ServiceLogger>(LoggerFactory.Create(config =>
                {
                    config.AddConsole();
                    config.SetMinimumLevel(LogLevel.Information);
                })));
            }

            return (service, settings);
        }

        /// <summary>
        /// The main application method from which the user interacts with all available methods.
        /// </summary>
        /// <param name="args">Command line arguments required to control check parameters.</param>
        private static void Main(string[] args)
        {
            (service, settings) = Configure(args);
            var commandHandler = CreateCommandHandler();
            Console.WriteLine($"\nFile Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
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