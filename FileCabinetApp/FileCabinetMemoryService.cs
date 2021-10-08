using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, IEnumerable<FileCabinetRecord>> cache = new (StringComparer.CurrentCultureIgnoreCase);

        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Record validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator) => this.validator = validator ?? throw new ArgumentNullException(nameof(validator), "Validator can't be null");

        /// <inheritdoc/>
        public RecordSnapshot MakeSnapshot() => new (this.GetRecords());

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record) => this.validator.ValidateRecord(record);

        /// <inheritdoc/>
        public void AddRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            if (this.IsExist(record.Id))
            {
                throw new ArgumentException("Record with this Id already exists");
            }

            this.recordList.Add(record);
            this.DictionaryAdd(record);
            this.cache.Clear();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.cache.TryGetValue(firstName, out var result))
            {
                return result;
            }

            if (this.firstNameDictionary.TryGetValue(firstName, out var records))
            {
                this.cache[firstName] = records;
                return this.cache[firstName];
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.cache.TryGetValue(lastName, out var result))
            {
                return result;
            }

            if (this.lastNameDictionary.TryGetValue(lastName, out var records))
            {
                this.cache[lastName] = records;
                return this.cache[lastName];
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth)
        {
            if (this.cache.TryGetValue(dateOfBirth, out var result))
            {
                return result;
            }

            if (DateTime.TryParse(dateOfBirth, out DateTime birthDate) && this.dateOfBirthDictionary.TryGetValue(birthDate, out var records))
            {
                this.cache[dateOfBirth] = records;
                return this.cache[dateOfBirth];
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public void EditRecord(FileCabinetRecord record)
        {
            this.ValidateInputRecord(record);

            if (!this.IsExist(record.Id))
            {
                throw new ArgumentException("Record with this Id already exists");
            }

            this.ValidateRecord(record);
            this.recordList.Add(record);
            this.DictionaryAdd(record);
            this.cache.Clear();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords() => this.GetRecords(new RecordQuery(_ => true, string.Empty));

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords(IRecordQuery query)
        {
            query ??= new RecordQuery(_ => true, string.Empty);

            if (this.cache.TryGetValue(query.QueryHashCode, out var result))
            {
                return result;
            }

            this.cache[query.QueryHashCode] = this.recordList.Where(record => query.Predicate(record));
            return this.cache[query.QueryHashCode];
        }

        /// <inheritdoc/>
        public (int AliveRecords, int DeletedRecords) GetStat()
        {
            return (this.recordList.Count, 0);
        }

        /// <inheritdoc/>
        public int Restore(RecordSnapshot snapshot)
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
                this.cache.Clear();
            }
        }

        /// <summary>
        /// An internal method that calls data updates in dictionaries.
        /// </summary>
        /// <param name="record">Record to add.</param>
        private void DictionaryAdd(FileCabinetRecord record)
        {
            if (this.firstNameDictionary.TryGetValue(record.FirstName, out var firstNameList))
            {
                firstNameList.Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(record.FirstName, new List<FileCabinetRecord> { record });
            }

            if (this.lastNameDictionary.TryGetValue(record.LastName, out var lastNameList))
            {
                lastNameList.Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(record.LastName, new List<FileCabinetRecord> { record });
            }

            if (this.dateOfBirthDictionary.TryGetValue(record.DateOfBirth, out var dateOfBirthList))
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

        private bool IsExist(int id) => this.recordList.Contains(new FileCabinetRecord { Id = id });

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