using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileCabinetApp;

/// <summary>
/// An abstract class that describes the general behavior for classes of descendants, which implements the main work with records.
/// </summary>
public abstract class FileCabinetMemoryService : IRecordValidator, IFileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

    /// <summary>
    /// Initialize a new <see cref="FileCabinetServiceShapshot"/> which contains <see cref="FileCabinetRecord"/> array.
    /// </summary>
    /// <returns>Returns <see cref="FileCabinetServiceShapshot"/> with data about existing records.</returns>
    public FileCabinetServiceShapshot MakeSnapshot()
    {
        return new FileCabinetServiceShapshot(this.list.ToArray());
    }

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordData recordData)
    {
        this.CreateValidator().ValidateParameters(recordData);
    }

    /// <summary>
    /// Create an instance of <see cref="IRecordValidator"/> and return it.
    /// </summary>
    /// <returns>Class wich realize <see cref="IRecordValidator"/> for calling .ValidateParameters() method.</returns>
    public abstract IRecordValidator CreateValidator();

    /// <summary>
    /// Create a new instance of <see cref="FileCabinetRecord"/> and save it to storage.
    /// </summary>
    /// <param name="recordData">Class "container" with data for new record.</param>
    /// <returns>Returns the unique identifier of the record.</returns>
    public int CreateRecord()
    {
        var dataContainer = new FileCabinetRecordData(this);
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
        if (this.firstNameDictionary.TryGetValue(firstName?.ToLower(Program.Culture), out List<FileCabinetRecord> recordList))
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
        if (this.lastNameDictionary.TryGetValue(lastName?.ToLower(Program.Culture), out List<FileCabinetRecord> recordList))
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
        var dataContainer = new FileCabinetRecordData(this);
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
    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        return this.list.AsReadOnly();
    }

    /// <inheritdoc/>
    public (int actualRecords, int deletedRecords) GetStat()
    {
        return (this.list.Count, 0);
    }

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceShapshot restoreSnapshot)
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
        firstName = firstName?.ToLower(Program.Culture);
        lastName = lastName?.ToLower(Program.Culture);

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
        List<FileCabinetRecord> firstNameList, lastNameList, dateOfBirthList;

        this.firstNameDictionary.TryGetValue(record.FirstName.ToLower(Program.Culture), out firstNameList);
        this.lastNameDictionary.TryGetValue(record.LastName.ToLower(Program.Culture), out lastNameList);
        this.dateOfBirthDictionary.TryGetValue(record.DateOfBirth, out dateOfBirthList);

        firstNameList.Remove(record);
        lastNameList.Remove(record);
        dateOfBirthList.Remove(record);
    }
}