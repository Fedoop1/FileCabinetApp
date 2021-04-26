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
    public class FileCabinetXMLReader
    {
        private XmlSerializer formatter;
        private XmlReader xmlReader;

        public FileCabinetXMLReader(FileStream fileStream)
        {
            this.formatter = new XmlSerializer(typeof(FileCabinetRecord[]));
            this.xmlReader = XmlReader.Create(fileStream);
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            var record = this.formatter.Deserialize(this.xmlReader) as FileCabinetRecord[];
            result.AddRange(record);
            return result;
        }
    }
}
