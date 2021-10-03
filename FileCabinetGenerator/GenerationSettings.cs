namespace FileCabinetGenerator
{
    /// <summary>
    /// Class "container" which storage and processing information for record generation and export.
    /// </summary>
    public class GenerationSettings
    {
        /// <summary>
        /// Gets and sets output type.
        /// </summary>
        public string OutputType { get; set; }

        /// <summary>
        /// Gets and sets file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets and sets record count.
        /// </summary>
        public int RecordsAmount { get; set; }

        /// <summary>
        /// Gets and sets start id.
        /// </summary>
        public int StartId { get; set; }
    }
}
