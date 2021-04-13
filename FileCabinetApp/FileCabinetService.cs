using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

    // Метод Input был добавлен для того, что бы не нарушить принцип DRY, а так же для сохранения гибкости и мастштабируемости программы.
    public (string firstName, string lastName, DateTime dateOfBirth, short height, decimal money, char gender) Input()
    {
        try
        {
            Console.WriteLine("\nFirst name: ");

            string firstName = Console.ReadLine();

            Console.WriteLine("Last name: ");

            string lastName = Console.ReadLine();

            Console.WriteLine("Date of birth: ");

            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }

            Console.WriteLine("Height: ");

            if (!short.TryParse(Console.ReadLine(), out short height))
            {
                throw new ArgumentException("Height is incorrect");
            }

            Console.WriteLine("Money: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal money))
            {
                throw new ArgumentException("Money is incorrect");
            }

            Console.WriteLine("Gender(M/F): ");

            if (!char.TryParse(Console.ReadLine(), out char gender))
            {
                throw new ArgumentException("Gender is incorrect");
            }

            return (firstName, lastName, dateOfBirth, height, money, gender);
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
            return this.Input();
        }
    }

    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal money, char gender)
    {
        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Height = height,
            Money = money,
            Gender = gender,
        };

        this.list.Add(record);
        this.DictionaryAdd(firstName, lastName, dateOfBirth, record);

        return record.Id;
    }

    public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short height, decimal money, char gender)
    {
        try
        {
            FileCabinetRecord editingRecord = this.list.FirstOrDefault(record => record.Id == id);

            if (editingRecord == null)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }

            this.firstNameDictionary.TryGetValue(editingRecord.FirstName.ToLower(Program.Culture), out List<FileCabinetRecord> firstNameList);
            this.lastNameDictionary.TryGetValue(editingRecord.LastName.ToLower(Program.Culture), out List<FileCabinetRecord> lastNameList);
            this.dateOfBirthDictionary.TryGetValue(editingRecord.DateOfBirth, out List<FileCabinetRecord> dateOfBirthList);

            editingRecord.FirstName = firstName;
            editingRecord.LastName = lastName;
            editingRecord.DateOfBirth = dateOfBirth;
            editingRecord.Height = height;
            editingRecord.Money = money;
            editingRecord.Gender = gender;

            firstNameList.Remove(editingRecord);
            lastNameList.Remove(editingRecord);
            dateOfBirthList.Remove(editingRecord);

            this.DictionaryAdd(firstName, lastName, dateOfBirth, editingRecord);

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

        var editData = this.Input();

        try
        {
            this.EditRecord(recordId, editData.firstName, editData.lastName, editData.dateOfBirth, editData.height, editData.money, editData.gender);
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
            this.EditRecord(id);
        }
    }

    public FileCabinetRecord[] GetRecords()
    {
        return this.list.ToArray();
    }

    public int GetStat()
    {
        return this.list.Count;
    }

    private void DictionaryAdd(string firstName, string lastName, DateTime dateOfBirth, FileCabinetRecord record)
    {
        firstName = firstName.ToLower(Program.Culture);
        lastName = lastName.ToLower(Program.Culture);

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