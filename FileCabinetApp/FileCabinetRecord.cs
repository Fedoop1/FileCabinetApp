using System;
using System.Xml.Serialization;

/// <summary>
/// A class describing the fields and behavior of a unit such as a record.
/// </summary>
[Serializable]
[XmlRoot]
public class FileCabinetRecord : IEquatable<FileCabinetRecord>
{
    /// <summary>
    /// Gets or sets the id field.
    /// </summary>
    /// <value>
    /// Id property.
    /// </value>
    [XmlAttribute("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the height field.
    /// </summary>
    /// <value>
    /// Height property.
    /// </value>
    [XmlElement("Height")]
    public short Height { get; set; }

    /// <summary>
    /// Gets or sets the money field.
    /// </summary>
    /// <value>
    /// Money property.
    /// </value>
    [XmlElement("Money")]
    public decimal Money { get; set; }

    /// <summary>
    /// Gets or sets the gender field.
    /// </summary>
    /// <value>
    /// Gender property.
    /// </value>
    [XmlElement("Gender")]
    public char Gender { get; set; }

    /// <summary>
    /// Gets or sets the first name field.
    /// </summary>
    /// <value>
    /// First name property.
    /// </value>
    [XmlAttribute("First")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name field.
    /// </summary>
    /// <value>
    /// Last name property.
    /// </value>
    [XmlAttribute("Last")]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth field.
    /// </summary>
    /// <value>
    /// Date of birth property.
    /// </value>
    [XmlElement("DateofBirth")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Overriding the ToString() method.
    /// </summary>
    /// <returns>Returns an overridden version of the ToString() method.</returns>
    public override string ToString()
    {
        return $"#{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth.Year}-{this.DateOfBirth.Month}-{this.DateOfBirth.Day}, {this.Height}, {this.Money}, {this.Gender}.";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return this.Equals((FileCabinetRecord)obj);
    }

    public bool Equals(FileCabinetRecord other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Id, this.Height, this.Money, this.Gender, this.FirstName, this.LastName, this.DateOfBirth);
    }
}