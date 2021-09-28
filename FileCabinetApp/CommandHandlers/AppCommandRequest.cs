namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// A request class which containing a command and parameters in it.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">User entered command.</param>
        /// <param name="parameters">Parameters to command.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the user required command value.
        /// </summary>
        /// <value>Command for further processing.
        /// </value>
        public string Command { get; }

        /// <summary>
        /// Gets the parameters to required command.
        /// </summary>
        /// <value>Parameters to command.
        /// </value>
        public string Parameters { get; }
    }
}
