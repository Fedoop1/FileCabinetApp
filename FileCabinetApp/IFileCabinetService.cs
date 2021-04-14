using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        public IRecordValidator CreateValidator();

        public int CreateRecord(FileCabinetRecordData recordData);

        public FileCabinetRecord[] FindByFirstName(string firstName);

        public FileCabinetRecord[] FindByLastName(string lastName);

        public FileCabinetRecord[] FindByDayOfBirth(string dateOfBirth);

        public void EditRecord(string id);

        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        public int GetStat();
    }
}
