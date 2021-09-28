namespace FileCabinetGenerator
{
    using System;

    /// <summary>
    /// Class "container" which storage and processing information for record generation and export.
    /// </summary>
    public class GeneratorCommandLineArgs
    {
        private string outputType;
        private string filePath;
        private int recordAmount;
        private int startId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorCommandLineArgs"/> class.
        /// </summary>
        /// <param name="outputType">The output type for future export.</param>
        /// <param name="filePath">File path where export will locate.</param>
        /// <param name="recordAmount">The count of <see cref="FileCabinetRecord"/> to generate.</param>
        /// <param name="startId">The first index that the record wull have.</param>
        public GeneratorCommandLineArgs(string outputType, string filePath, int recordAmount, int startId)
        {
            this.OutputType = outputType;
            this.FilePath = filePath;
            this.RecordAmount = recordAmount;
            this.StartId = startId;
        }

        /// <summary>
        /// Gets and sets output type.
        /// </summary>
        public string OutputType
        {
            get => this.outputType;
            private set
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Length == 3)
                {
                    this.outputType = value;
                    return;
                }

                throw new ArgumentException("Output type is null or incorrect.");
            }
        }

        /// <summary>
        /// Gets and sets filepath.
        /// </summary>
        public string FilePath
        {
            get => this.filePath;
            private set
            {
                if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrEmpty(value))
                {
                    this.filePath = value;
                    return;
                }

                throw new ArgumentException("File path is null or empty.");
            }
        }

        /// <summary>
        /// Gets and sets record count.
        /// </summary>
        public int RecordAmount
        {
            get => this.recordAmount;
            private set
            {
                if (value >= 0)
                {
                    this.recordAmount = value;
                    return;
                }

                throw new ArgumentException("Record amount lower than zero.");
            }
        }

        /// <summary>
        /// Gets and sets start id.
        /// </summary>
        public int StartId
        {
            get => this.startId;
            private set
            {
                if (value >= 0)
                {
                    this.startId = value;
                    return;
                }

                throw new ArgumentException("Start id lower than zero.");
            }
        }
    }
}
