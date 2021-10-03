using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validate the incoming amount of money.
    /// </summary>
    public class MoneyValidator : IRecordValidator
    {
        /// <summary>
        /// The minimum amount of money.
        /// </summary>
        private readonly decimal minMoney;

        private readonly decimal maxMoney;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyValidator"/> class.
        /// </summary>
        /// <param name="minMoney">The minimal amount of money.</param>
        public MoneyValidator(decimal minMoney)
        {
            this.minMoney = minMoney;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyValidator"/> class.
        /// </summary>
        /// <param name="minMoney">The minimum amount of money.</param>
        /// <param name="maxMoney">The maximum amount of money.</param>
        public MoneyValidator(decimal minMoney, decimal maxMoney)
        {
            this.minMoney = minMoney;
            this.maxMoney = maxMoney;
        }

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record data is null");
            }

            return record.Money > this.minMoney && record.Money < this.maxMoney;
        }
    }
}
