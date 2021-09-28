using System;

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

        public MoneyValidator(decimal minMoney, decimal maxMoney)
        {
            this.minMoney = minMoney;
            this.maxMoney = maxMoney;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (recordData.Money < this.minMoney || recordData.Money > this.maxMoney)
            {
                throw new ArgumentException("Invalid money amount.");
            }
        }
    }
}
