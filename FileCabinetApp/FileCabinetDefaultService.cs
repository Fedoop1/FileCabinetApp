﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetDefaultService : FileCabinetService
    {
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
