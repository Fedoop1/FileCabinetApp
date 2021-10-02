using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Decorators;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private static IServiceProvider services;

        private static bool isRunning = true;

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                Console.WriteLine(item);
            }
        }

        private static ICommandHandler CreateCommandHandler()
        {
            var createHandler = new InsertCommandHandler(services.GetService<IFileCabinetService>());
            var editHandler = new UpdateCommandHandler(services.GetService<IFileCabinetService>());
            var exitHandler = new ExitCommandHandler(services.GetService<IFileCabinetService>(), UpdateApplicationStatus);
            var exportHandler = new ExportCommandHandler(services.GetService<IFileCabinetService>(), services);
            var findHandler = new FindCommandHandler(services.GetService<IFileCabinetService>(), DefaultRecordPrint);
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(services.GetService<IFileCabinetService>(), services.GetService<IRecordSnapshotService>());
            var listHandler = new ListCommandHandler(services.GetService<IFileCabinetService>(), DefaultRecordPrint);
            var missedHandler = new MissedCommandHandler();
            var purgeHandler = new PurgeCommandHandler(services.GetService<IFileCabinetService>());
            var removeHandler = new DeleteCommandHandler(services.GetService<IFileCabinetService>());
            var statHandler = new StatCommandHandler(services.GetService<IFileCabinetService>());

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
        private static void Configure(string[] parameters)
        {
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
        }

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection result = new ServiceCollection()
                .AddSingleton(typeof(ILoggerFactory), LoggerFactory.Create(config =>
                {
                    config.AddConsole();
                    config.SetMinimumLevel(LogLevel.Information);
                }))
                .AddTransient(typeof(ILogger), service => service.GetService<ILoggerFactory>().CreateLogger("FileCabinetLogger"))
                .AddSingleton(typeof(IValidationSettings), _ =>
                {
                    return configuration["validation-rules"] == "custom" || configuration["v"] == "custom"
                        ? configuration.GetSection("Custom").Get<ValidationSettings>() : configuration.GetSection("Default").Get<ValidationSettings>();
                })
                .AddSingleton(typeof(IRecordValidator), service => ValidatorBuilder.CreateValidator(service.GetService<IValidationSettings>()))
                .AddSingleton(typeof(IFileCabinetService), service =>
                {
                    var validator = service.GetService<IRecordValidator>();
                    IFileCabinetService fileService = configuration["storage"] == "file" || configuration["s"] == "file"
                        ? new FileCabinetFilesystemService("cabinet-records.db", validator)
                        : new FileCabinetMemoryService(validator);

                    if (bool.TryParse(configuration["use-stopwatch"], out var useServiceMeter) && useServiceMeter)
                    {
                        fileService = new ServiceMeter(fileService);
                    }

                    if (bool.TryParse(configuration["use-logger"], out bool useLogger) && useLogger)
                    {
                        fileService = new ServiceLogger(fileService, service.GetService<ILogger>());
                    }

                    return fileService;
                })
                .AddTransient(typeof(IRecordSnapshotService),
                    service =>
                    {
                        var result = new FileCabinetSnapshotService(service.GetService<IFileCabinetService>());
                        result.AddDataSaver("xml", filepath => new FileCabinetRecordXmlLWriter(filepath));
                        result.AddDataSaver("csv", filepath => new FileCabinetRecordCSVWriter(filepath));
                        result.AddDataLoader("xml", filepath => new FileCabinetXMLReader(filepath));
                        result.AddDataLoader("csv", filepath => new FileCabinetCsvReader(filepath));

                        return result;
                    });

            return result.BuildServiceProvider();
        }

        /// <summary>
        /// The main application method from which the user interacts with all available methods.
        /// </summary>
        /// <param name="args">Command line arguments required to control check parameters.</param>
        private static void Main(string[] args)
        {
            Configure(args);
            services = ConfigureServices();
            var commandHandler = CreateCommandHandler();
            Console.Write("$FileCabinetApp.exe");

            foreach (var parameter in args)
            {
                Console.WriteLine(parameter);
            }

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