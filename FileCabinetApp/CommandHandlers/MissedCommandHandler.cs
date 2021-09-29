using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle <see cref="AppCommandRequest"/> if no one of the handlers was able to process the request.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
                return;
            }

            PrintMissedCommandInfo(commandRequest?.Command);
        }

        /// <summary>
        /// A method that is executed if a non-existent command is selected.
        /// </summary>
        /// <param name="command">User invoked command.</param>
        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
