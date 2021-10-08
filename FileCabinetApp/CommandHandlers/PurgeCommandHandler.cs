using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler which process 'Purge' operation.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Record service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override string Command => "purge";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command == "purge")
            {
                this.Purge();
                return;
            }

            if (this.NextHandle != null)
            {
                this.NextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Compresses and clean up deleted data.
        /// </summary>
        private void Purge() => Console.WriteLine(this.Service.Purge());
    }
}
