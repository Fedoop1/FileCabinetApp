using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Сomposite class that collects all the necessary validations rules inside and validate incoming data.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly IEnumerable<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">List of <see cref="IRecordValidator"/> which the input data will be checked.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(recordData);
            }
        }
    }
}
