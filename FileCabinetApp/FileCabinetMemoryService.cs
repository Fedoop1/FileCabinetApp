using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Validators;

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

        private readonly IValidationSettings validationSettings;

        public FileCabinetMemoryService(IValidationSettings settings) => this.validationSettings = settings ?? throw new ArgumentNullException(nameof(settings), "Validation settings can't be null");

        /// <summary>
        /// Initialize a new <see cref="FileCabinetServiceSnapshot"/> which contains <see cref="FileCabinetRecord"/> array.
        /// </summary>
        /// <returns>Returns <see cref="FileCabinetServiceSnapshot"/> with data about existing records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.recordList.ToArray());
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            this.CreateValidator().ValidateParameters(recordData);
        }

        /// <summary>
        /// Create an instance of <see cref="IRecordValidator"/> and return it.
        /// </summary>
        /// <returns>Class which realize <see cref="IRecordValidator"/> for calling .ValidateParameters() method.</returns>
        public IRecordValidator CreateValidator() => ValidatorBuilder.CreateValidator(this.validationSettings);

        /// <summary>
        /// Create a new instance of <see cref="FileCabinetRecord"/> and save it to storage.
        /// </summary>
        /// <returns>Returns the unique identifier of the record.</returns>
        public int CreateRecord()
        {
            var dataContainer = new FileCabinetRecordData(this.validationSettings);
            dataContainer.InputData();
            this.ValidateParameters(dataContainer);

            var record = new FileCabinetRecord
            {
                Id = this.recordList.Count + 1,
                FirstName = dataContainer.FirstName,
                LastName = dataContainer.LastName,
                DateOfBirth = dataContainer.DateOfBirth,
                Height = dataContainer.Height,
                Money = dataContainer.Money,
                Gender = dataContainer.Gender,
            };

            this.recordList.Add(record);
            this.DictionaryAdd(record);

            return record.Id;
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

        private static IEnumerable<FileCabinetRecord> RecordsIterator(IEnumerable<FileCabinetRecord> source)
        {
            foreach (var record in source)
            {
                yield return record;
            }
        }

        /// <inheritdoc/>
        public void EditRecord(int id)
        {
            if (!this.RemoveRecord(id))
            {
                Console.WriteLine("Record doesn't exist.");
                return;
            }

            var dataContainer = new FileCabinetRecordData(this.validationSettings);
            dataContainer.InputData();
            this.ValidateParameters(dataContainer);

            var newRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = dataContainer.FirstName,
                LastName = dataContainer.LastName,
                DateOfBirth = dataContainer.DateOfBirth,
                Height = dataContainer.Height,
                Money = dataContainer.Money,
                Gender = dataContainer.Gender,
            };

            this.recordList.Add(newRecord);
            this.DictionaryAdd(newRecord);

            Console.WriteLine($"Record #{id} is updated.");
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
        public void Restore(FileCabinetServiceSnapshot restoreSnapshot)
        {
            if (restoreSnapshot is null)
            {
                throw new ArgumentNullException(nameof(restoreSnapshot), "Restore snapshot is null");
            }

            var restoreRecordList = restoreSnapshot.Records;
            foreach (var restoreRecord in restoreRecordList)
            {
                if (this.recordList.TryGetValue(restoreRecord, out var oldValue))
                {
                    this.recordList.Remove(oldValue);
                    this.DictionaryRemove(oldValue);
                    Console.WriteLine($"Record #{restoreRecord.Id} was updated!");
                }

                this.recordList.Add(restoreRecord);
                this.DictionaryAdd(restoreRecord);
            }
        }

        /// <inheritdoc/>
        public string Purge()
        {
            return "The operation is not supported for the application memory mode.";
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int index)
        {
            var record = this.recordList.FirstOrDefault(rec => rec.Id == index);

            if (record is null)
            {
                return false;
            }

            this.recordList.Remove(record);
            this.DictionaryRemove(record);
            return true;
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
    }
}