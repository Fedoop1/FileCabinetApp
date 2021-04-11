﻿using System;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Nikita Malukov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService = new FileCabinetService();
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the count if records", "The 'stat' command prints the count of the records." },
            new string[] { "create", "create the record in file cabinet", "The 'create' command create the record in file cabinet." },
            new string[] { "list", "prints the list if records", "The 'list' command prints the list of the records." },
        };

        private static void Create(string parameters)
        {
            Console.WriteLine("First name: ");

            string firstName = Console.ReadLine();

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("incorrect first name");
            }

            Console.WriteLine("Last name: ");

            string lastName = Console.ReadLine();

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("incorrect last name");
            }

            Console.WriteLine("Date of birth: ");

            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
            {
                throw new ArgumentException("incorrect date of birth");
            }

            Console.WriteLine("Field1: ");

            if (!short.TryParse(Console.ReadLine(), out short field1))
            {
                throw new ArgumentException("field1 is incorrect");
            }

            Console.WriteLine("Field2: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal field2))
            {
                throw new ArgumentException("field2 is incorrect");
            }

            Console.WriteLine("Field3: ");

            if (!char.TryParse(Console.ReadLine(), out char field3))
            {
                throw new ArgumentException("field2 is incorrect");
            }

            int result = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, field1, field2, field3);

            Console.WriteLine(
                $"First name: {firstName}\nLast name: {lastName}\nDate of birth: {dateOfBirth.ToShortDateString()}\nRecord #{result} is created.");
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
                var inputs = Console.ReadLine().Split(' ', 2);
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
}