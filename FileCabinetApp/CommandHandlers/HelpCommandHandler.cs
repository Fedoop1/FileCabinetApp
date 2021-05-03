using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class HelpCommandHandler : CommandHadlerBase
    {
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }
        }
    }
}
