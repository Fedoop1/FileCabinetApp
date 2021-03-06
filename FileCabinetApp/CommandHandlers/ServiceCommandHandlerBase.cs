using FileCabinetApp.Interfaces;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Fields should be private

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class implementing the <see cref="ICommandHandler"/>
    /// and is the basic class for all <see cref="ServiceCommandHandlerBase"/> inheritance. Contains a protected field on the <see cref="IFileCabinetService"/> context.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// <see cref="IFileCabinetService"/> context required for the correct operation of the methods.
        /// </summary>
        protected readonly IFileCabinetService Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">A context that will be passed to all inheritors from the abstract class for the correct execution of commands.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.Service = service;
        }

        /// <summary>
        /// Gets the command processed by the handler.
        /// </summary>
        /// <value>
        /// The processed command.
        /// </value>
        public abstract string Command { get; }
    }
}
