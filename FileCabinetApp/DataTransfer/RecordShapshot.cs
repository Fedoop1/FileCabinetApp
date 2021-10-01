using System.Collections.Generic;

namespace FileCabinetApp
{
    public class RecordShapshot
    {
        public RecordShapshot(IEnumerable<FileCabinetRecord> records) => this.Records = records;

        public IEnumerable<FileCabinetRecord> Records { get; }
    }
}
