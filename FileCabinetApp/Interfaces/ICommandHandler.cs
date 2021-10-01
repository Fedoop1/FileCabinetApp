using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface which describe basic behavior to command handler classes that handle user input commands.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Set pointer for the next <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="commandHandler">Next command hanlder.</param>
        void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Try to handle command request. Othewise pass it further along the chain.
        /// </summary>
        /// <param name="commandRequest">Command with parameters.</param>
        void Handle(AppCommandRequest commandRequest);
    }
}
