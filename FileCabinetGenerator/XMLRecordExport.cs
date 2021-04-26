// <copyright file="XMLRecordExport.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileCabinetGenerator
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Static class wich serialize data array into XML file.
    /// </summary>
    public static class XMLRecordExport
    {
        /// <summary>
        /// Serialize <see cref="FileCabinetRecord"/> array into XML format.
        /// </summary>
        /// <param name="fileStream"><see cref="FileStream"/> with information about file for exporting.</param>
        /// <param name="recordArray">Array of <see cref="FileCabinetRecord"/> with information about records.</param>
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
