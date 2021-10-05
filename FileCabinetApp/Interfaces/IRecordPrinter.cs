using System.Collections.Generic;
using System.Reflection;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface provides method to printer providers which write sequence of <see cref="FileCabinetRecord"/> to destination stream.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints source to destination stream.
        /// </summary>
        /// <param name="source">Records source.</param>
        public void Print(IEnumerable<FileCabinetRecord> source);

        /// <summary>
        /// Prints source to destination file with using specified select method according to selected fields.
        /// </summary>
        /// <param name="source">Records source.</param>
        /// <param name="selectedFields">Collection of selected fields.</param>
        public void Print(IEnumerable<FileCabinetRecord> source, IEnumerable<PropertyInfo> selectedFields);
    }
}
