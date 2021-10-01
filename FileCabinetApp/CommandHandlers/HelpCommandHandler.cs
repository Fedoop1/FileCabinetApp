﻿using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "help" command from user input.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// The jagged array needed to get help with all the available commands consists of the command name, a short description, and an extended description.
        /// </summary>
        private static readonly string[][] HelpMessages = new string[][]
        {
            new[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new[] { "stat", "prints the count if records", "The 'stat' command prints the count of the records." },
            new[] { "create", "create the record in file cabinet", "The 'create' command create the record in file cabinet." },
            new[] { "list", "prints the list if records", "The 'list' command prints the list of the records." },
            new[] { "edit", "edits the record", "The 'edit' command edits the value of the record." },
            new[] { "find", "finds a record", "The 'find' command find a record by the specified parameter. Example '>find [param] [data]." },
            new[] { "export", "Make snapshot and save it to file.", "The export command make snapshot of you record list and save it to special file." },
            new[] { "import", "Import records from external storage.", "The import command imports records from a file in two possible formats XML and CSV." },
            new[] { "remove", "Remove selected record.", "The command remove record at the selected index." },
            new[] { "purge", "Defragment the db file.", "The command invokes an algorithm that destroys deleted records from the file." },
        };

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "help")
            {
                PrintHelp(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Method displays all auxiliary information about existing methods, or additional information about a specific method.
        /// </summary>
        /// <param name="parameters">Parameter with the choice of a specific command to display help about it.</param>
        private static void PrintHelp(string parameters)
        {
            const int CommandHelpIndex = 0;
            const int DescriptionHelpIndex = 1;
            const int ExplanationHelpIndex = 2;

            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }
        }
    }
}
