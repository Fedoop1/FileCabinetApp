﻿using System;
using System.Globalization;

public class FileCabinetRecord
{
    private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
    private short height;
    private decimal money;
    private char gender;
    private string firstName;
    private string lastName;
    private DateTime dateOfBirth;

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

    public int Id { get; set; }

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

    public DateTime DateOfBirth
    {
        get
        {
            return this.dateOfBirth;
        }

        set
        {
            if (value >= DateTime.Parse("01.12.1950", Culture) && value <= DateTime.Now)
            {
                this.dateOfBirth = value;
            }
            else
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }
        }
    }

    public override string ToString()
    {
        return $"#{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth.Year}-{this.DateOfBirth.Month}-{this.DateOfBirth.Day}, {this.Height}, {this.Money}, {this.Gender}.";
    }
}