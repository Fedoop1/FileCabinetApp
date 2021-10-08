using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class that composite all the necessary validations rules inside and validate record according to them.
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
        public bool ValidateRecord(FileCabinetRecord record) => this.validators.All(validator => validator.ValidateRecord(record));
    }
}
