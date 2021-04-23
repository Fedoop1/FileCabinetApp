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
    public int CreateRecord(FileCabinetRecordData recordData)
    {
        this.ValidateParameters(recordData);

        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = recordData?.FirstName,
            LastName = recordData.LastName,
            DateOfBirth = recordData.DateOfBirth,
            Height = recordData.Height,
            Money = recordData.Money,
            Gender = recordData.Gender,
        };

        this.list.Add(record);
        this.DictionaryAdd(recordData.FirstName, recordData.LastName, recordData.DateOfBirth, record);

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
    public void EditRecord(string id, FileCabinetRecordData newData)
    {
        if (!int.TryParse(id, out int recordId))
        {
            Console.WriteLine($"Id is incorrect.");
            return;
        }

        FileCabinetRecord record = this.list.FirstOrDefault(x => x.Id == recordId);

        if (record == null)
        {
            Console.WriteLine($"#{id} record is not found.");
            return;
        }

        this.ValidateParameters(newData);

        this.firstNameDictionary.TryGetValue(record.FirstName.ToLower(Program.Culture), out List<FileCabinetRecord> firstNameList);
        this.lastNameDictionary.TryGetValue(record.LastName.ToLower(Program.Culture), out List<FileCabinetRecord> lastNameList);
        this.dateOfBirthDictionary.TryGetValue(record.DateOfBirth, out List<FileCabinetRecord> dateOfBirthList);

        record.FirstName = newData?.FirstName;
        record.LastName = newData.LastName;
        record.DateOfBirth = newData.DateOfBirth;
        record.Height = newData.Height;
        record.Money = newData.Money;
        record.Gender = newData.Gender;

        firstNameList.Remove(record);
        lastNameList.Remove(record);
        dateOfBirthList.Remove(record);

        this.DictionaryAdd(newData.FirstName, newData.LastName, newData.DateOfBirth, record);

        Console.WriteLine($"Record #{id} is updated.");
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        return this.list.AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetStat()
    {
        return this.list.Count;
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
}