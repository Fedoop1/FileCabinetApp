using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// A class that implements standard rules for data validation.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetMemoryService
    {
        /// <inheritdoc/>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
