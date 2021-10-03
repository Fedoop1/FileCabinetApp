using FileCabinetApp.Interfaces;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1306 // Field names should begin with lower-case letter
#pragma warning disable SA1401 // Fields should be private

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
        protected ICommandHandler NextHandle;

        /// <inheritdoc/>
        public abstract void Handle(AppCommandRequest commandRequest);

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.NextHandle = commandHandler;
        }
    }
}
