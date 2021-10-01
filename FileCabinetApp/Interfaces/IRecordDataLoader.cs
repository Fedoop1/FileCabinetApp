using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    public interface IRecordDataLoader
    {
        public IEnumerable<FileCabinetRecord> Load();
    }
}
