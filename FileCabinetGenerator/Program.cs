// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Globalization;
    using System.IO;
    using FileCabinetApp;

    public static class Program
    {
        public static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
        private static GeneratorCommandLineArgs commandLineArgs;

        private static GeneratorCommandLineArgs HandlingCommandLineArgs(string[] parameters)
        {
            string outputType = string.Empty;
            string filePath = string.Empty;
            int recordAmount = 0;
            int startId = 0;

            Console.Write("$FileCabinetGenerator.exe");

            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                Console.Write(" " + parameters[parameterIndex]);
                switch (parameters[parameterIndex])
                {
                    case string atribute when atribute.Contains("output-type"):
                        atribute = atribute.Substring(atribute.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
                        if (atribute.ToUpperInvariant() == "CSV")
                        {
                            outputType = "CSV";
                            break;
                        }
                        else if (atribute.ToUpperInvariant() == "XML")
                        {
                            outputType = "XML";
                            break;
                        }

                        throw new ArgumentException("Output-type is incorrect.");
                    case "-t":
                        if (parameterIndex + 1 != parameters.Length)
                        {
                            outputType = parameters[parameterIndex + 1].ToUpperInvariant();
                            break;
                        }

                        throw new ArgumentException("Output-type is incorrect.");
                    case "-o":
                        if (parameterIndex + 1 != parameters.Length)
                        {
                            filePath = parameters[parameterIndex + 1];
                            break;
                        }

                        throw new ArgumentException("Output file name is incorrect.");
                    case string atribute when atribute.Contains("output"):
                        filePath = atribute.Substring(atribute.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
                        break;
                    case string atribute when atribute.Contains("records-amount"):
                        atribute = atribute.Substring(atribute.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
                        if (!int.TryParse(atribute, out recordAmount))
                        {
                            throw new ArgumentException("Records amout is incorrect.");
                        }

                        break;
                    case "-a":
                        if (parameterIndex + 1 != parameters.Length)
                        {
                            if (int.TryParse(parameters[parameterIndex] + 1, out recordAmount))
                            {
                                break;
                            }
                        }

                        throw new ArgumentException("Records amout is incorrect.");
                    case string atribute when atribute.Contains("start-id"):
                        atribute = atribute.Substring(atribute.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
                        if (!int.TryParse(atribute, out startId))
                        {
                            throw new ArgumentException("Start id is incorrect.");
                        }

                        break;
                    case "-i":
                        if (parameterIndex + 1 != parameters.Length)
                        {
                            if (int.TryParse(parameters[parameterIndex] + 1, out startId))
                            {
                                break;
                            }
                        }

                        throw new ArgumentException("Records amout is incorrect.");
                    default: throw new ArgumentException("Command line arg doesn't exists!");
                }
            }

            return new GeneratorCommandLineArgs(outputType, filePath, recordAmount, startId);
        }

        private static void Export(GeneratorCommandLineArgs commandLineArgs, FileCabinetRecord[] fileCabinetRecords)
        {
            try
            {
                switch (commandLineArgs.OutputType)
                {
                    case "CSV":
                        CSVRecordExport.Export(commandLineArgs.FilePath, fileCabinetRecords);
                        break;
                    case "XML":
                        XMLRecordExport.Export(commandLineArgs.FilePath, fileCabinetRecords);
                        break;
                    default: throw new ArgumentException("Output type is doesn't exists.");
                }

                Console.WriteLine($"\n{commandLineArgs.RecordAmount} records were written to {commandLineArgs.FilePath}.");
            }
            catch (IOException)
            {
                Console.WriteLine($"\nCan't open file {commandLineArgs.FilePath}");
                return;
            }
        }

        private static void Main(string[] args)
        {
            commandLineArgs = HandlingCommandLineArgs(args);
            FileCabinetRecord[] recordGeneratedArray = RecordGenerator.GenerateRecord(commandLineArgs.StartId, commandLineArgs.RecordAmount);
            Export(commandLineArgs, recordGeneratedArray);
        }
    }
}
