using System;
using FileCabinetApp;

/// <summary>
/// A class describing the fields and behavior of a unit such as a record.
/// </summary>
[Serializable]
public class FileCabinetRecord
{
    private short height;
    private decimal money;
    private char gender;
    private string firstName;
    private string lastName;
    private DateTime dateOfBirth;

    /// <summary>
    /// Height property.
    /// </summary>
    /// <value>
    /// Gets and sets the height field.
    /// </value>
    public short Height
    {
        get
        {
            return this.height;
        }

        set
        {
            if (value >= 10 && value <= short.MaxValue)
            {
                this.height = value;
            }
            else
            {
                throw new ArgumentException("Height is incorrect.");
            }
        }
    }

    /// <summary>
    /// Money property.
    /// </summary>
    /// <value>
    /// Gets and sets the money field.
    /// </value>
    public decimal Money
    {
        get
        {
            return this.money;
        }

        set
        {
            if (value >= 0)
            {
                this.money = value;
            }
            else
            {
                throw new ArgumentException("Money can't be lower than zero.");
            }
        }
    }

    /// <summary>
    /// Gender property.
    /// </summary>
    /// <value>
    /// Gets and sets the gender field.
    /// </value>
    public char Gender
    {
        get
        {
            return this.gender;
        }

        set
        {
            if (value == 'M' || value == 'm' || value == 'F' || value == 'f')
            {
                this.gender = value;
            }
            else
            {
                throw new ArgumentException("Gender is incorrect.");
            }
        }
    }

    /// <summary>
    /// Id property.
    /// </summary>
    /// <value>
    /// Gets and sets the id field.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// First name property.
    /// </summary>
    /// <value>
    /// Gets and sets the first name field.
    /// </value>
    public string FirstName
    {
        get
        {
            return this.firstName;
        }

        set
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length >= 2 && value.Length <= 60)
            {
                this.firstName = value;
            }
            else
            {
                throw new ArgumentException("First name is incorrect.");
            }
        }
    }

    /// <summary>
    /// Last name property.
    /// </summary>
    /// <value>
    /// Gets and sets the last name field.
    /// </value>
    public string LastName
    {
        get
        {
            return this.lastName;
        }

        set
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length >= 2 && value.Length <= 60)
            {
                this.lastName = value;
            }
            else
            {
                throw new ArgumentException("Last name is incorrect.");
            }
        }
    }

    /// <summary>
    /// Date of birth property.
    /// </summary>
    /// <value>
    /// Gets and sets the date of birth field.
    /// </value>
    public DateTime DateOfBirth
    {
        get
        {
            return this.dateOfBirth;
        }

        set
        {
            if (value >= DateTime.Parse("01.12.1950", Program.Culture) && value <= DateTime.Now)
            {
                this.dateOfBirth = value;
            }
            else
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }
        }
    }

    /// <summary>
    /// Overriding the ToString() method.
    /// </summary>
    /// <returns>Returns an overridden version of the ToString() method.</returns>
    public override string ToString()
    {
        return $"#{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth.Year}-{this.DateOfBirth.Month}-{this.DateOfBirth.Day}, {this.Height}, {this.Money}, {this.Gender}.";
    }
}