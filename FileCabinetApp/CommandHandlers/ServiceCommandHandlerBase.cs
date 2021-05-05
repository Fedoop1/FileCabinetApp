using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class implementing the <see cref="ICommandHandler"/>
    /// and is the basic class for all <see cref="ServiceCommandHandlerBase"/> inheritance. Contains a protected field on the <see cref="IFileCabinetService"/> context.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHadlerBase
    {
        /// <summary>
        /// <see cref="IFileCabinetService"/> context required for the correct operation of the methods.
        /// </summary>
#pragma warning disable SA1401 // Do not declare visible instance fields
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected IFileCabinetService service;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Do not declare visible instance fields

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">A context that will be passed to all inheritors from the abstract class for the correct execution of commands.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.service = service;
        }
    }
}
