using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    public class HeightValidator : IRecordValidator
    {
        /// <summary>
        /// The maximum value is the height of growth.
        /// </summary>
        private readonly short maxHeight;

        /// <summary>
        /// The minimum value is the height of the growth.
        /// </summary>
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
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (recordData.Height < this.minHeight || recordData.Height > this.maxHeight)
            {
                throw new ArgumentException("Height is incorrect.");
            }
        }
    }
}
