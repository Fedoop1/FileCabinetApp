namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// The interface required to validate the parameters in accordance with the selected rules.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the record be specified rules.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Result of validating.</returns>
        bool ValidateRecord(FileCabinetRecord record);
    }
}
