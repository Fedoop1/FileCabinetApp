using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class implementing the <see cref="ICommandHandler"/> and is the basic class for all <see cref="CommandHandlerBase"/> inheritance.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Pointer for the next <see cref="ICommandHandler"/>.
        /// </summary>
        protected ICommandHandler nextHandle;

        /// <inheritdoc/>
        public abstract void Handle(AppCommandRequest commandRequest);

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandle = commandHandler;
        }
    }
}
