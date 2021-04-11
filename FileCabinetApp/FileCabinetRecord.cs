using System;

public class FileCabinetRecord
{
    private short field1;
    private decimal field2;
    private char field3;

    public short Field1
    {
        get { return this.field1; }
        set { this.field1 = value; }
    }

    public decimal Field2
    {
        get { return this.field2; }
        set { this.field2 = value; }
    }

    public char Field3
    {
        get { return this.field3; }
        set { this.field3 = value; }
    }

    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public override string ToString()
    {
        return $"#{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth.Year}-{this.DateOfBirth.Month}-{this.DateOfBirth.Day}, {this.field1}, {this.field2}, {this.field3}.";
    }
}