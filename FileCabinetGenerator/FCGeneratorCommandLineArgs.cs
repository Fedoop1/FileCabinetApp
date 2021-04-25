﻿using System;

namespace FileCabinetGenerator
{
    public class FCGeneratorCommandLineArgs
    {
        private string outputType;
        private string filePath;
        private int recordAmount;
        private int startId;

        public FCGeneratorCommandLineArgs(string outputType, string filePath, int recordAmount, int startId)
        {
            this.OutputType = outputType;
            this.FilePath = filePath;
            this.RecordAmount = recordAmount;
            this.StartId = startId;
        }

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
