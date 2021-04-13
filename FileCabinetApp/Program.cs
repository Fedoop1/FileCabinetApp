﻿using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        public static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
        private const string DeveloperName = "Nikita Malukov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService = new FileCabinetDefaultService();
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("-v", ChangeValidationRules),
            new Tuple<string, Action<string>>("--validation-rules", ChangeValidationRules),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the count if records", "The 'stat' command prints the count of the records." },
            new string[] { "create", "create the record in file cabinet", "The 'create' command create the record in file cabinet." },
            new string[] { "list", "prints the list if records", "The 'list' command prints the list of the records." },
            new string[] { "edit", "edits the record", "The 'edit' command edits the value of the record." },
            new string[] { "find", "finds a record", "The 'find' command find a record by the specified parameter. Example '>find [param] [data]." },
            new string[] { "--validation-rules", "changes the type of check rules", "The '--validation-rules' or '-v' changes the type of check rules." },
        };

        private static void ChangeValidationRules(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Empty flag.");
                return;
            }

            switch (parameters.ToLower(Culture))
            {
                case "default":
                    fileCabinetService = new FileCabinetDefaultService();
                    Console.WriteLine("Using default validation rules.");
                    Console.WriteLine(HintMessage);
                    return;
                case "custom":
                    fileCabinetService = new FileCabinetCustomService();
                    Console.WriteLine("Using custom validation rules.");
                    Console.WriteLine(HintMessage);
                    return;
                default:
                    Console.WriteLine("Unknown flag.");
                    Console.WriteLine(HintMessage);
                    return;
            }
        }

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
            try
            {
                var recordData = new FileCabinetRecordData();
                recordData.InputData();
                int result = fileCabinetService.CreateRecord(recordData);

                Console.WriteLine(
                $"Record #{result} is created.");
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
                Create(parameters);
                return;
            }
        }

        private static void Edit(string parameters)
        {
            try
            {
                fileCabinetService.EditRecord(parameters);
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void List(string parameters)
        {
            var recordsArray = fileCabinetService.GetRecords();

            foreach (var record in recordsArray)
            {
                Console.WriteLine(record);
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");

                string inputCommand = Console.ReadLine();
                var inputs = inputCommand.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) switch
                {
                    -1 => inputCommand.Split(" ", 2),
                    _ => inputCommand.Split(":", 2),
                };

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

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

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

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }

    // test
}