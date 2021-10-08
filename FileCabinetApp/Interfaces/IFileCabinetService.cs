using System;
using System.Collections.Generic;
using FileCabinetApp.DataTransfer;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// The interface includes all the methods common to the classes inherited from FileCabinetService.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// The method that creates a new FileCabinetRecord.
        /// </summary>
        public void AddRecord(FileCabinetRecord record);

        /// <summary>
        /// A method that searches for a record by name.
        /// </summary>
        /// <param name="firstName">A parameter consisting of the first name.</param>
        /// <returns>Returns an array of all records matching the search term.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// A method that searches for a record by second name.
        /// </summary>
        /// <param name="lastName">A parameter consisting of the last name.</param>
        /// <returns>Returns an array of all records matching the search term.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// A method that searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">A parameter consisting of the date of birth.</param>
        /// <returns>Returns an array of all records matching the search term.</returns>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth);

        /// <summary>
        /// A method for editing data in a particular record.
        /// </summary>
        /// <param name="record">Updated record.</param>
        public void EditRecord(FileCabinetRecord record);

        /// <summary>
        /// A method that returns a collection of records.
        /// </summary>
        /// <returns>Sequence of records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// A method that return the collection of records selected by <see cref="IRecordQuery"/>.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Sequence of records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(IRecordQuery query);

        /// <summary>
        /// A method that returns actual records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public RecordSnapshot MakeSnapshot();

        /// <summary>
        /// Restore information from <see cref="FileCabinetSnapshotService"/> instance.
        /// </summary>
        /// <param name="restoreSnapshot">Snapshot with information about records.</param>
        /// <returns>Count of affected records.</returns>
        public int Restore(RecordSnapshot restoreSnapshot);

        /// <summary>
        /// Get the count of existing and deleted <see cref="FileCabinetRecord"/>'s.
        /// </summary>
        /// <returns>The count of the records.</returns>
        public (int AliveRecords, int DeletedRecords) GetStat();

        /// <summary>
        /// Removes a record from a data source.
        /// </summary>
        /// <param name="record">Record to delete.</param>
        public void DeleteRecord(FileCabinetRecord record);

        /// <summary>
        /// Find and remove record from data source file by index.
        /// </summary>
        /// <returns>Removal result.</returns>
        public string Purge();
    }
}
