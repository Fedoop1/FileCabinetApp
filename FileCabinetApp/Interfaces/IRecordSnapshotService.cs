using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    public interface IRecordSnapshotService
    {
        public IEnumerable<FileCabinetRecord> Records { get;}

        public void SaveTo(string fileFormat, string filePath, bool append);

        public void LoadFrom(string fileFormat, string filePath);
    }
}
