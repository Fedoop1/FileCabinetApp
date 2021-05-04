using FileCabinetApp.Validators;
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

        /// <inheritdoc/>
        public override IRecordValidator CreateValidator()
        {
            return ValidatorBuilder.CreateCustom();
        }
    }
}
