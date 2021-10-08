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
        /// Adds the record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void AddRecord(FileCabinetRecord record);

        /// <summary>
        /// Finds the record by first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>The sequence of records that satisfy the condition.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds the record by last name.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>The sequence of records that satisfy the condition.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds the record by day of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>The sequence of records that satisfy the condition.</returns>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth);

        /// <summary>
        /// Editing data in the record.
        /// </summary>
        /// <param name="record">Updated record.</param>
        public void EditRecord(FileCabinetRecord record);

        /// <summary>
        /// Returns sequence of stored records.
        /// </summary>
        /// <returns>Sequence of records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns the collection of records selected by <see cref="IRecordQuery"/>.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sequence of records that satisfy the condition.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(IRecordQuery query);

        /// <summary>
        /// Returns actual records in <see cref="RecordSnapshot"/> format.
        /// </summary>
        /// <returns>Service snapshot.</returns>
        public RecordSnapshot MakeSnapshot();

        /// <summary>
        /// Restores the specified records from service snapshot.
        /// </summary>
        /// <param name="restoreSnapshot">The restore snapshot.</param>
        /// <returns>Count of restored records.</returns>
        public int Restore(RecordSnapshot restoreSnapshot);

        /// <summary>
        /// Get the count of existing and deleted <see cref="FileCabinetRecord"/>'s.
        /// </summary>
        /// <returns>The count of existing and deleted records.</returns>
        public (int AliveRecords, int DeletedRecords) GetStat();

        /// <summary>
        /// Removes a record from service.
        /// </summary>
        /// <param name="record">Record to delete.</param>
        public void DeleteRecord(FileCabinetRecord record);

        /// <summary>
        /// Optimize records memory by cleaning deleted records and defragmentate memory into one contiguous block.
        /// </summary>
        /// <returns>Removal result.</returns>
        public string Purge();
    }
}
