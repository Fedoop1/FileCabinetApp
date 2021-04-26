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
        public static void Export(FileStream fileStream, FileCabinetRecord[] recordArray)
        {
            if (recordArray is null)
            {
                throw new ArgumentNullException(nameof(recordArray), "Array of records is null");
            }

            XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecord[]));
            formatter.Serialize(fileStream, recordArray);
        }
    }
}
