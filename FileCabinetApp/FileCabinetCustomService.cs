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
    public class FileCabinetCustomService : FileCabinetMemoryService
    {
        private readonly FileCabinetRecordData dataContainer = new FileCabinetRecordData("custom");

        /// <inheritdoc/>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
