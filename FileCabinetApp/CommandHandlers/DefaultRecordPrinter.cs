using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class DefaultRecordPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> records)
        {

            foreach (var item in records ?? Array.Empty<FileCabinetRecord>())
            {
                Console.WriteLine(item);
            }
        }
    }
}
