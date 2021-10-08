using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class which validate incoming height value.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private readonly short maxHeight;
        private readonly short minHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">The minimum value is the height of the growth.</param>
        /// <param name="maxHeight">The maximum value is the height of growth.</param>
        public HeightValidator(short minHeight, short maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record data is null");
            }

            return record.Height > this.minHeight && record.Height < this.maxHeight;
        }
    }
}
