using System;
using System.Collections.Generic;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Class which work with records in memory and provide all operations for this.
    /// </summary>
    public class FileCabinetMemoryService : IRecordValidator, IFileCabinetService
    {
        private readonly HashSet<FileCabinetRecord> recordList = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new (StringComparer.CurrentCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new (StringComparer.CurrentCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        private readonly IRecordValidator validator;

        public FileCabinetMemoryService(IRecordValidator validator) => this.validator = validator ?? throw new ArgumentNullException(nameof(validator), "Validator can't be null");

        /// <summary>
        /// Initialize a new <see cref="FileCabinetSnapshotService"/> which contains <see cref="FileCabinetRecord"/> array.
        /// </summary>
        /// <returns>Returns <see cref="FileCabinetSnapshotService"/> with data about existing records.</returns>
        public RecordShapshot MakeSnapshot() => new (this.GetRecords());

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record) => this.validator.ValidateRecord(record);

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetRecord"/> and save it to storage.
        /// </summary>
        public void AddRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            if (this.isExist(record.Id))
            {
                throw new ArgumentException("Record with this Id already exists");
            }

            this.recordList.Add(record);
            this.DictionaryAdd(record);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName ?? string.Empty, out List<FileCabinetRecord> recordList))
            {
                return RecordsIterator(recordList);
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName ?? string.Empty, out List<FileCabinetRecord> recordList))
            {
                return RecordsIterator(recordList);
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, out DateTime birthDate) && this.dateOfBirthDictionary.TryGetValue(birthDate, out List<FileCabinetRecord> recordList))
            {
                return RecordsIterator(recordList);
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public void EditRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            if (!this.isExist(record.Id))
            {
                throw new ArgumentException("Record with this Id already exists");
            }

            this.ValidateRecord(record);
            this.recordList.Add(record);
            this.DictionaryAdd(record);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var record in this.recordList)
            {
                yield return record;
            }
        }

        /// <inheritdoc/>
        public (int AliveRecords, int DeletedRecords) GetStat()
        {
            return (this.recordList.Count, 0);
        }

        /// <inheritdoc/>
        public int Restore(RecordShapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), "Restore snapshot is null");
            }

            int affectedRecordsCount = default;
            foreach (var restoreRecord in snapshot.Records)
            {
                if (this.recordList.TryGetValue(restoreRecord, out var oldValue))
                {
                    this.recordList.Remove(oldValue);
                    this.DictionaryRemove(oldValue);
                }

                this.recordList.Add(restoreRecord);
                this.DictionaryAdd(restoreRecord);
                affectedRecordsCount++;
            }

            return affectedRecordsCount;
        }

        /// <inheritdoc/>
        public string Purge()
        {
            return "The operation is not supported for the application memory mode.";
        }

        /// <inheritdoc/>
        public void DeleteRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null");
            }

            if (this.recordList.TryGetValue(record, out var actualValue))
            {
                this.recordList.Remove(actualValue);
                this.DictionaryRemove(actualValue);
            }
        }

        /// <summary>
        /// An internal method that calls data updates in dictionaries.
        /// </summary>
        /// <param name="record">Record to add.</param>
        private void DictionaryAdd(FileCabinetRecord record)
        {
            if (this.firstNameDictionary.TryGetValue(record.FirstName, out List<FileCabinetRecord> firstNameList))
            {
                firstNameList.Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(record.FirstName, new List<FileCabinetRecord> { record });
            }

            if (this.lastNameDictionary.TryGetValue(record.LastName, out List<FileCabinetRecord> lastNameList))
            {
                lastNameList.Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(record.LastName, new List<FileCabinetRecord> { record });
            }

            if (this.dateOfBirthDictionary.TryGetValue(record.DateOfBirth, out List<FileCabinetRecord> dateOfBirthList))
            {
                dateOfBirthList.Add(record);
            }
            else
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// An internal method that calls data deletion in dictionaries.
        /// </summary>
        /// <param name="record">The record to be removed from the dictionaries.</param>
        private void DictionaryRemove(FileCabinetRecord record)
        {
            this.firstNameDictionary[record.FirstName].Remove(record);
            this.lastNameDictionary[record.LastName].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
        }

        private static IEnumerable<FileCabinetRecord> RecordsIterator(IEnumerable<FileCabinetRecord> source)
        {
            foreach (var record in source)
            {
                yield return record;
            }
        }

        private bool isExist(int id) => this.recordList.Contains(new FileCabinetRecord { Id = id });

        private void ValidateInputRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null");
            }

            if (!this.ValidateRecord(record))
            {
                throw new ArgumentException("Record data doesn't according validation rules");
            }
        }
    }
}