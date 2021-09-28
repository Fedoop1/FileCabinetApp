using System;
using System.Xml.Serialization;

/// <summary>
/// A class describing the fields and behavior of a unit such as a record.
/// </summary>
[Serializable]
[XmlRoot]
public class FileCabinetRecord
{
    /// <summary>
    /// Gets or sets the id field.
    /// </summary>
    /// <value>
    /// Id property.
    /// </value>
    [XmlAttribute]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the height field.
    /// </summary>
    /// <value>
    /// Height property.
    /// </value>
    [XmlElement]
    public short Height { get; set; }

    /// <summary>
    /// Gets or sets the money field.
    /// </summary>
    /// <value>
    /// Money property.
    /// </value>
    [XmlElement]
    public decimal Money { get; set; }

        /// <summary>
    /// Gets or sets the gender field.
    /// </summary>
    /// <value>
    /// Gender property.
    /// </value>
    [XmlElement]
    public char Gender { get; set; }

    /// <summary>
    /// Gets or sets the first name field.
    /// </summary>
    /// <value>
    /// First name property.
    /// </value>
    [XmlElement("First-Name")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name field.
    /// </summary>
    /// <value>
    /// Last name property.
    /// </value>
    [XmlElement("Last-Name")]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth field.
    /// </summary>
    /// <value>
    /// Date of birth property.
    /// </value>
    [XmlElement("Date-of-Birth")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Overriding the ToString() method.
    /// </summary>
    /// <returns>Returns an overridden version of the ToString() method.</returns>
    public override string ToString()
    {
        return $"#{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth.Year}-{this.DateOfBirth.Month}-{this.DateOfBirth.Day}, {this.Height}, {this.Money}, {this.Gender}.";
    }
}