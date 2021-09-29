using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Class which work with records in memory and provide all operations for this.
    /// </summary>
    public class FileCabinetMemoryService : IRecordValidator, IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        // ReSharper disable once MemberCanBePrivate.Global
        private readonly IValidationSettings validationSettings;

        public FileCabinetMemoryService(IValidationSettings settings) => this.validationSettings = settings ?? throw new ArgumentNullException(nameof(settings), "Validation settings can't be null");

        /// <summary>
        /// Initialize a new <see cref="FileCabinetServiceSnapshot"/> which contains <see cref="FileCabinetRecord"/> array.
        /// </summary>
        /// <returns>Returns <see cref="FileCabinetServiceSnapshot"/> with data about existing records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
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
                Id = this.list.Count + 1,
                FirstName = dataContainer.FirstName,
                LastName = dataContainer.LastName,
                DateOfBirth = dataContainer.DateOfBirth,
                Height = dataContainer.Height,
                Money = dataContainer.Money,
                Gender = dataContainer.Gender,
            };

            this.list.Add(record);
            this.DictionaryAdd(dataContainer.FirstName, dataContainer.LastName, dataContainer.DateOfBirth, record);

            return record.Id;
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName?.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> recordList))
            {
                return recordList.ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName?.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> recordList))
            {
                return recordList.ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        /// <inheritdoc/>
        public FileCabinetRecord[] FindByDayOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, out DateTime birthDate))
            {
                if (this.dateOfBirthDictionary.TryGetValue(birthDate, out List<FileCabinetRecord> recordList))
                {
                    return recordList.ToArray();
                }
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public void EditRecord(int id)
        {
            var dataContainer = new FileCabinetRecordData(this.validationSettings);
            dataContainer.InputData();
            this.ValidateParameters(dataContainer);

            if (!this.RemoveRecord(id))
            {
                return;
            }

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

            this.list.Add(newRecord);
            this.DictionaryAdd(dataContainer.FirstName, dataContainer.LastName, dataContainer.DateOfBirth, newRecord);

            Console.WriteLine($"Record #{id} is updated.");
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.AsReadOnly();
        }

        /// <inheritdoc/>
        public (int RecordsCount, int DeletedRecords) GetStat()
        {
            return (this.list.Count, 0);
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
                if (this.list.Any(record => record.Id == restoreRecord.Id))
                {
                    var oldRecord = this.list.First(record => record.Id == restoreRecord.Id);
                    this.list.Remove(oldRecord);
                    this.DictionaryRemove(oldRecord);
                    Console.WriteLine($"Record #{restoreRecord.Id} was updated!");
                }

                this.list.Add(restoreRecord);
                this.DictionaryAdd(restoreRecord.FirstName, restoreRecord.LastName, restoreRecord.DateOfBirth, restoreRecord);
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
            var record = this.list.FirstOrDefault(rec => rec.Id == index);

            if (record is null)
            {
                return false;
            }

            this.list.Remove(record);
            this.DictionaryRemove(record);
            return true;
        }

        /// <summary>
        /// An internal method that calls data updates in dictionaries.
        /// </summary>
        /// <param name="firstName">New name.</param>
        /// <param name="lastName">New surname.</param>
        /// <param name="dateOfBirth">New date of birth.</param>
        /// <param name="record">A record corresponding to these parameters.</param>
        private void DictionaryAdd(string firstName, string lastName, DateTime dateOfBirth, FileCabinetRecord record)
        {
            firstName = firstName?.ToLower(CultureInfo.CurrentCulture);
            lastName = lastName?.ToLower(CultureInfo.CurrentCulture);

            if (this.firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> firstNamefList))
            {
                firstNamefList.Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>() { record });
            }

            if (this.lastNameDictionary.TryGetValue(lastName, out List<FileCabinetRecord> lastNameList))
            {
                lastNameList.Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>() { record });
            }

            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out List<FileCabinetRecord> dateOfBirthList))
            {
                dateOfBirthList.Add(record);
            }
            else
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>() { record });
            }
        }

        /// <summary>
        /// An internal method that calls data deletion in dictionaries.
        /// </summary>
        /// <param name="record">The record to be removed from the dictionaries.</param>
        private void DictionaryRemove(FileCabinetRecord record)
        {
            this.firstNameDictionary.TryGetValue(record.FirstName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> firstNameList);
            this.lastNameDictionary.TryGetValue(record.LastName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> lastNameList);
            this.dateOfBirthDictionary.TryGetValue(record.DateOfBirth, out List<FileCabinetRecord> dateOfBirthList);

            firstNameList.Remove(record);
            lastNameList.Remove(record);
            dateOfBirthList.Remove(record);
        }
    }
}