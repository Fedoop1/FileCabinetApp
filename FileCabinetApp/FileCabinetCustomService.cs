using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// A class that implements custom rules for data validation.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService, IFileCabinetService
    {
        /// <inheritdoc/>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
