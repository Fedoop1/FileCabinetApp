using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "exit" command from user input.
    /// </summary>
    public class ExitCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<bool> exitDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="service"><see cref="IFileCabinetService"/> context required for the correct operation of the methods.</param>
        /// <param name="exitDelegate">Delegate method to remotely control application running status.</param>
        public ExitCommandHandler(IFileCabinetService service, Action<bool> exitDelegate)
            : base(service)
        {
            this.exitDelegate = exitDelegate;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "exit")
            {
                this.Exit();
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// A method that produces a safe exit from the application.
        /// </summary>
        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.exitDelegate(false);
        }

        public override string Command => "exit";
    }
}
