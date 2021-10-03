namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// The interface required to validate the parameters in accordance with the selected rules.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// A method that invokes parameter validation through the class that implements this interface.
        /// </summary>
        /// <param name="record">The container class includes all the information about the record.</param>
        bool ValidateRecord(FileCabinetRecord record);
    }
}
