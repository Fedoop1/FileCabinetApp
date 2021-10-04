using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    public interface IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> source);
    }
}
