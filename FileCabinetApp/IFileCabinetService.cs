using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// The interface includes all the methods common to the classes inherited from FileCabinetService.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// A method that creates an instance of the IRecordValidator interface required for data validation.
        /// </summary>
        /// <returns>Returns an instance of the IRecordValidator interface.</returns>
        public IRecordValidator CreateValidator();

        /// <summary>
        /// The method that creates a new FileCabinetRecord.
        /// </summary>
        /// <param name="recordData">The "container" class with all the information about the record.</param>
        /// <returns>Returns the unique identifier of the record.</returns>
        public int CreateRecord(FileCabinetRecordData recordData);

        /// <summary>
        /// A method that searches for a record by name.
        /// </summary>
        /// <param name="firstName">A parameter consisting of the first name.</param>
        /// <returns>Returns an array of all records matching the search term.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName);

        /// <summary>
        /// A method that searches for a record by second name.
        /// </summary>
        /// <param name="lastName">A parameter consisting of the last name.</param>
        /// <returns>Returns an array of all records matching the search term.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName);

        /// <summary>
        /// A method that searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">A parameter consisting of the date of birth.</param>
        /// <returns>Returns an array of all records matching the search term.</returns>
        public FileCabinetRecord[] FindByDayOfBirth(string dateOfBirth);

        /// <summary>
        /// A method for editing data in a particular record.
        /// </summary>
        /// <param name="id">A unique identifier by which the editing will take place.</param>
        /// <param name="recordData">A container class with updated record information.</param>
        public void EditRecord(int id, FileCabinetRecordData recordData);

        /// <summary>
        /// A method that returns a collection of records (read-only).
        /// </summary>
        /// <returns>Collection of records (read-only).</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// A method that returns the count of records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public FileCabinetServiceShapshot MakeSnapshot();

        /// <summary>
        /// Restore information from <see cref="FileCabinetServiceShapshot"/> instance.
        /// </summary>
        /// <param name="restoreSnapshot">Snapshot with information about records.</param>
        public void Restore(FileCabinetServiceShapshot restoreSnapshot);

        /// <summary>
        /// Get the count of existing <see cref="FileCabinetRecord"/>'s.
        /// </summary>
        /// <returns>The count of the records.</returns>
        public int GetStat();

        public bool RemoveRecord(int index);

        public string Purge();
    }
}
