using System;

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
            new[] { "help", "Prints the help screen", "The 'help' command prints the help screen." },
            new[] { "exit", "Exits the application", "The 'exit' command close the application." },
            new[] { "stat", "Prints the stat of records", "The 'stat' command prints the stat of the file cabinet service." },
            new[] { "insert", "Insert a new record", "The 'insert' command insert a new record to the file cabinet service. Example '>insert (id, firstname, lastname, dateofbirth) values ('1', 'John', 'Doe', '5/18/1986').'" },
            new[] { "update", "Update the record", "The 'update' command updates the value of the record. Example 'update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'\r\n'" },
            new[] { "export", "Make snapshot and save it to file.", "The export command makes a snapshot of you records and saves it to a special file." },
            new[] { "import", "Import records from the external storage.", "The import command imports records from a destination file according by specified format." },
            new[] { "delete", "Delete selected record.", "The command delete record by the specified parameter. Example >delete where id = '1'." },
            new[] { "purge", "Fragmentate database file.", "The command invokes an algorithm that removes deleted records from the destination file and clear the space." },
        };

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("help", StringComparison.CurrentCultureIgnoreCase))
            {
                PrintHelp(commandRequest.Parameters);
                return;
            }

            if (this.NextHandle != null)
            {
                this.NextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Method displays all auxiliary information about existing methods, or additional information about a specific method.
        /// </summary>
        /// <param name="parameters">Parameter with the choice of a specific command to display help about it.</param>
        private static void PrintHelp(string parameters)
        {
            const int commandHelpIndex = 0;
            const int descriptionHelpIndex = 1;
            const int explanationHelpIndex = 2;

            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[commandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][explanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[commandHelpIndex], helpMessage[descriptionHelpIndex]);
                }
            }
        }
    }
}
