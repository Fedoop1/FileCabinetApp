using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle <see cref="AppCommandRequest"/> if no one of the handlers was able to process the request.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        private readonly IEnumerable<string> availableCommands;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissedCommandHandler"/> class.
        /// </summary>
        /// <param name="availableCommands">The available commands.</param>
        public MissedCommandHandler(IEnumerable<string> availableCommands) => this.availableCommands = availableCommands;

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
                return;
            }

            this.PrintMissedCommandInfo(commandRequest?.Command);
        }

        /// <summary>
        /// A method that is executed if a non-existent command is selected.
        /// </summary>
        /// <param name="command">User invoked command.</param>
        private void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command. Print 'help [command]'\n");
            Console.WriteLine($"The most similar command is:\n\t{string.Join("\n\t", this.FindSimilarCommands(command))}");
            Console.WriteLine();
        }

        private IEnumerable<string> FindSimilarCommands(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return this.availableCommands;
            }

            return this.availableCommands.Where(cmd =>
                cmd.StartsWith(command, StringComparison.CurrentCultureIgnoreCase) || command.StartsWith(cmd[0].ToString(), StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
