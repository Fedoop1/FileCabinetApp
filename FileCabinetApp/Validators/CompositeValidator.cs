using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    public class CompositeValidator : IRecordValidator
    {
        private IEnumerable<IRecordValidator> validators;

        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(recordData);
            }
        }
    }
}
