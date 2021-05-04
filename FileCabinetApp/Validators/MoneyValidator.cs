using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    public class MoneyValidator : IRecordValidator
    {
        /// <summary>
        /// The minimum amount of money.
        /// </summary>
        private decimal minMoney;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyValidator"/> class.
        /// </summary>
        /// <param name="minMoney">The mininal amount of money.</param>
        public MoneyValidator(decimal minMoney)
        {
            this.minMoney = minMoney;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (recordData?.Money < this.minMoney)
            {
                throw new ArgumentException("Money can't be lower than zero.");
            }
        }
    }
}
