using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for working and deserializing information from an XML file.
    /// </summary>
    public class FileCabinetXMLReader
    {
        private readonly XmlSerializer formatter;
        private readonly XmlReader xmlReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXMLReader"/> class.
        /// </summary>
        /// <param name="fileStream"><see cref="FileStream"/> with information about XML format file.</param>
        public FileCabinetXMLReader(FileStream fileStream)
        {
            this.formatter = new XmlSerializer(typeof(FileCabinetRecord[]));
            this.xmlReader = XmlReader.Create(fileStream);
        }

        /// <summary>
        /// Read and deserialize all information from file and return it <see cref="IList{FileCabinetRecord}"/> representation.
        /// </summary>
        /// <returns><see cref="IList{FileCabinetRecord}"/> representation of redords into XML file.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var result = new List<FileCabinetRecord>();

            var record = this.formatter.Deserialize(this.xmlReader) as FileCabinetRecord[];
            result.AddRange(record);
            return result;
        }
    }
}
