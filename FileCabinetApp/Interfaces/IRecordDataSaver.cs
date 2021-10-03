using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    public interface IRecordDataSaver
    {
        public void Save(IEnumerable<FileCabinetRecord> source, bool append);
    }
}
