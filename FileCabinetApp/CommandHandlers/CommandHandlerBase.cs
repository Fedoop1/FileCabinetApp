using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class implementing the <see cref="ICommandHandler"/> and is the basic class for all <see cref="CommandHandlerBase"/> inheritance.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
#pragma warning disable SA1401 // Do not declare visible instance fields
#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// Pointer for the next <see cref="ICommandHandler"/>.
        /// </summary>
        protected ICommandHandler nextHandle;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Do not declare visible instance fields

        /// <inheritdoc/>
        public abstract void Handle(AppCommandRequest commandRequest);

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandle = commandHandler;
        }
    }
}
