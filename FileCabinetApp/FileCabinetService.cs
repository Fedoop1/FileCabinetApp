using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileCabinetApp;

public abstract class FileCabinetService : IRecordValidator, IFileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

    public void ValidateParameters(FileCabinetRecordData recordData)
    {
        this.CreateValidator().ValidateParameters(recordData);
    }

    public abstract IRecordValidator CreateValidator();

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

    public void EditRecord(int id, FileCabinetRecordData recordData)
    {
        try
        {
            FileCabinetRecord editingRecord = this.list.FirstOrDefault(record => record.Id == id);

            if (editingRecord == null)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }

            try
            {
                this.ValidateParameters(recordData);
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message);
            }

            this.firstNameDictionary.TryGetValue(editingRecord.FirstName.ToLower(Program.Culture), out List<FileCabinetRecord> firstNameList);
            this.lastNameDictionary.TryGetValue(editingRecord.LastName.ToLower(Program.Culture), out List<FileCabinetRecord> lastNameList);
            this.dateOfBirthDictionary.TryGetValue(editingRecord.DateOfBirth, out List<FileCabinetRecord> dateOfBirthList);

            editingRecord.FirstName = recordData?.FirstName;
            editingRecord.LastName = recordData.LastName;
            editingRecord.DateOfBirth = recordData.DateOfBirth;
            editingRecord.Height = recordData.Height;
            editingRecord.Money = recordData.Money;
            editingRecord.Gender = recordData.Gender;

            firstNameList.Remove(editingRecord);
            lastNameList.Remove(editingRecord);
            dateOfBirthList.Remove(editingRecord);

            this.DictionaryAdd(recordData.FirstName, recordData.LastName, recordData.DateOfBirth, editingRecord);

            Console.WriteLine($"Record #{id} is updated.");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
            this.EditRecord(id.ToString(Program.Culture));
        }
    }

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

    public void EditRecord(string id)
    {
        if (!int.TryParse(id, out int recordId))
        {
            throw new ArgumentException($"Id is incorrect.");
        }

        FileCabinetRecord record = this.list.FirstOrDefault(x => x.Id == recordId);

        if (record == null)
        {
            throw new ArgumentException($"#{id} record is not found.");
        }

        FileCabinetRecordData newData = new FileCabinetRecordData();
        newData.InputData();

        this.EditRecord(recordId, newData);
    }

    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        return this.list.AsReadOnly();
    }

    public int GetStat()
    {
        return this.list.Count;
    }

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