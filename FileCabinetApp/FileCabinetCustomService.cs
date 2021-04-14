using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetCustomService : FileCabinetService, IFileCabinetService
    {
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
