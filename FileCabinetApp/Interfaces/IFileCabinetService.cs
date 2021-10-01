﻿using System.Collections.Generic;
using FileCabinetApp.DataTransfer;

namespace FileCabinetApp.Interfaces
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
        /// <returns>Returns the unique identifier of the record.</returns>
        public int CreateRecord();

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
        /// <param name="id">A unique identifier by which the editing will take place.</param>
        public bool EditRecord(int id);

        /// <summary>
        /// A method that returns a collection of records (read-only).
        /// </summary>
        /// <returns>Collection of records (read-only).</returns>
        public IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// A method that returns actual records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public RecordShapshot MakeSnapshot();

        /// <summary>
        /// Restore information from <see cref="FileCabinetSnapshotService"/> instance.
        /// </summary>
        /// <param name="restoreSnapshot">Snapshot with information about records.</param>
        public int Restore(RecordShapshot restoreSnapshot);

        /// <summary>
        /// Get the count of existing and deleted <see cref="FileCabinetRecord"/>'s.
        /// </summary>
        /// <returns>The count of the records.</returns>
        public (int AliveRecords, int DeletedRecords) GetStat();

        /// <summary>
        /// Removes a record from a data source.
        /// </summary>
        /// <param name="index">The identifier of the record to be deleted.</param>
        /// <returns>Removal result.</returns>
        public bool RemoveRecord(int index);

        /// <summary>
        /// Find and remove record from data source file by index.
        /// </summary>
        /// <returns>Removal result.</returns>
        public string Purge();
    }
}