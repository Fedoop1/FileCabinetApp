using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Decorators;
using FileCabinetApp.Interfaces;
using FileCabinetApp.RecordPrinters;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1116 // Split parameters should start on line after declaration

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Nikita Malukov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static IConfiguration configuration;
        private static IServiceProvider services;

        private static bool isRunning = true;

        private static ICommandHandler CreateAndSetCommandHandlers()
        {
            CommandHandlerBase[] handlers =
            {
                new InsertCommandHandler(services.GetService<IFileCabinetService>()),
                new UpdateCommandHandler(services.GetService<IFileCabinetService>()),
                new ExitCommandHandler(services.GetService<IFileCabinetService>(), UpdateApplicationStatus),
                new ExportCommandHandler(services.GetService<IFileCabinetService>(), services.GetService<IRecordSnapshotService>()),
                new HelpCommandHandler(),
                new ImportCommandHandler(services.GetService<IFileCabinetService>(), services.GetService<IRecordSnapshotService>()),
                new PurgeCommandHandler(services.GetService<IFileCabinetService>()),
                new DeleteCommandHandler(services.GetService<IFileCabinetService>()),
                new StatCommandHandler(services.GetService<IFileCabinetService>()),
                new SelectCommandHandler(services.GetService<IFileCabinetService>(), services.GetService<IRecordPrinter>()),

                // Additional cell to MissedCommandHandler, it's always last.
                null,
            };

            handlers[^1] = new MissedCommandHandler(GetAvailableCommands(handlers));

            SetupHandlersChain(handlers);

            const int firstHandlerInChain = 0;
            return handlers[firstHandlerInChain];

            static void SetupHandlersChain(IList<CommandHandlerBase> source)
            {
                for (int index = 0; index < source.Count - 1; index++)
                {
                    source[index].SetNext(source[index + 1]);
                }
            }

            static IEnumerable<string> GetAvailableCommands(IEnumerable<CommandHandlerBase> source)
            {
                foreach (var handler in source.OfType<ServiceCommandHandlerBase>())
                {
                    yield return handler.Command;
                }
            }
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
                    ["-v"] = "validation-rules",
                    ["-s"] = "storage",
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
                .AddTransient(typeof(ILogger), service => service.GetService<ILoggerFactory>() !.CreateLogger("FileCabinetLogger"))
                .AddSingleton(typeof(IRecordPrinter), _ => new TablePrinter(Console.Out))
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
                .AddSingleton(typeof(IRecordSnapshotService),
                    service =>
                    {
                        var result = new FileCabinetSnapshotService(service.GetService<IFileCabinetService>());
                        result.AddDataSaver("xml", filepath => new FileCabinetRecordXmlLWriter(filepath));
                        result.AddDataSaver("csv", filepath => new FileCabinetRecordCsvWriter(filepath));
                        result.AddDataLoader("xml", filepath => new FileCabinetXmlReader(filepath));
                        result.AddDataLoader("csv", filepath => new FileCabinetCsvReader(filepath));

                        return result;
                    });

            return result.BuildServiceProvider();
        }

        private static void Main(string[] args)
        {
            Configure(args);
            services = ConfigureServices();
            var firstCommandHandler = CreateAndSetCommandHandlers();
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
                var inputs = Console.ReadLine() !.Split(" ", 2);

                const int commandIndex = 0;
                const int parametersIndex = 1;
                string parameters = inputs.Length > parametersIndex ? inputs[parametersIndex] : string.Empty;
                firstCommandHandler.Handle(new AppCommandRequest(inputs[commandIndex], parameters));
            }
            while (isRunning);
        }
    }
}