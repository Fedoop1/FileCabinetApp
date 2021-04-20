using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    public class FileCabinetRecordXMLWriter
    {
        private XmlWriter xmlWriter;

        public FileCabinetRecordXMLWriter(StreamWriter writer)
        {
            this.xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings() { Indent = true });
        }

        public void Write(FileCabinetRecord[] fileCabinetRecordsArray)
        {
            if (fileCabinetRecordsArray?.Length == 0)
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement root = document.CreateElement("records");
            document.AppendChild(root);

            foreach (var record in fileCabinetRecordsArray)
            {
                XmlElement rootElement = document.CreateElement("record");
                rootElement.SetAttribute("id", $"{record.Id}");
                root.AppendChild(rootElement);
                XmlElement nameElement = document.CreateElement("name");
                nameElement.SetAttribute("first", record.FirstName);
                nameElement.SetAttribute("last", record.LastName);
                rootElement.AppendChild(nameElement);
                XmlElement dateOfBirthElement = document.CreateElement("dateOfBirth");
                dateOfBirthElement.InnerText = record.DateOfBirth.ToShortDateString();
                XmlElement height = document.CreateElement("height");
                height.InnerText = $"{record.Height}";
                XmlElement gender = document.CreateElement("gender");
                gender.InnerText = $"{record.Gender}";
                XmlElement money = document.CreateElement("money");
                money.InnerText = $"{record.Money}";
                rootElement.AppendChild(dateOfBirthElement);
                rootElement.AppendChild(height);
                rootElement.AppendChild(gender);
                rootElement.AppendChild(money);
            }

            document.Save(this.xmlWriter);
            this.xmlWriter.Close();
        }
    }
}
