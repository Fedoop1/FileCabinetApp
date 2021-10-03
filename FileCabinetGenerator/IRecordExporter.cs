using System.Collections.Generic;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public interface IRecordExporter
    {
        public void Export(IEnumerable<FileCabinetRecord> source);
    }
}
