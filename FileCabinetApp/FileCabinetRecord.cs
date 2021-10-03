using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
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

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj) => obj is FileCabinetRecord record && this.Equals(record);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(FileCabinetRecord other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id == other.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => this.Id;
    }
}