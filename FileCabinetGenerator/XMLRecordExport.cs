// <copyright file="XMLRecordExport.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileCabinetGenerator
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public static class XMLRecordExport
    {
        public static void Export(string filePath, FileCabinetRecord[] recordArray)
        {
            if (recordArray is null)
            {
                throw new ArgumentNullException(nameof(recordArray), "Array of records is null");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecord));

                foreach (var record in recordArray)
                {
                    formatter.Serialize(fileStream, record);
                }
            }
        }
    }
}
