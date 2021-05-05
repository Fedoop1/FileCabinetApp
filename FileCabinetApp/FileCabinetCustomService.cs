using FileCabinetApp.Validators;

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
