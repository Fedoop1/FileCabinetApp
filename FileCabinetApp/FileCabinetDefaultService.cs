using System;
using FileCabinetApp.Validators;

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
            return ValidatorBuilder.CreateDefault();
        }
    }
}
