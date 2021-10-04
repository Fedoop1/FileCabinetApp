using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.RecordPrinters
{
    public class DefaultPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> source)
        {
            if (source is null)
            {
                Console.WriteLine("Source is empty");
                return;
            }

            foreach (var record in source)
            {
                Console.WriteLine(record);
            }
        }
    }
}
