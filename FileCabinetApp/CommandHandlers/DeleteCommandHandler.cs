﻿using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "Delete" command to import data from file in special format.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const int KeyIndex = 0;
        private const int ValueIndex = 1;
        private const int ParametersCount = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IFileCabinetService"/> context is necessary for the correct execution of the methods.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Delete(commandRequest.Parameters);
                return;
            }

            if (this.nextHandle != null)
            {
                this.nextHandle.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Removes a record from a data source.
        /// </summary>
        /// <param name="parameters">The identifier of the record to be deleted.</param>
        private void Delete(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    Console.WriteLine("Index is null or empty!");
                    return;
                }

                var parametersArray = parameters.ToLowerInvariant().Split(new[] { "where" },
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parametersArray.Length != ParametersCount)
                {
                    Console.WriteLine("Invalid parameters count");
                }

                var pair = ExtractKeyValuePair(parametersArray[0]);
                var predicate = GeneratePredicate(pair.key, pair.value);

                List<int> deletedRecordsId = new ();
                foreach (var record in this.Service.GetRecords().Where(record => predicate(record)))
                {
                    this.Service.DeleteRecord(record);
                    deletedRecordsId.Add(record.Id);
                }

                if (deletedRecordsId.Count == 0)
                {
                    Console.WriteLine("There isn't record to delete");
                    return;
                }

                Console.WriteLine($"Record(s) {string.Join(", ", deletedRecordsId.Select(record => $"#{record}")).TrimEnd(new[] { ',', ' ' })} are deleted.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"During deleting an error was happened. Error message: {exception.Message}.");
            }
        }

        private static (string key, string value) ExtractKeyValuePair(string parameter)
        {
            var pair = parameter.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (pair.Length != 2)
            {
                throw new ArgumentException("Invalid key-value pair");
            }

            return (pair[KeyIndex], pair[ValueIndex].Trim('\u0027'));
        }

        private static Predicate<FileCabinetRecord> GeneratePredicate(string key, string value)
        {
            var property =
                typeof(FileCabinetRecord).GetProperties().FirstOrDefault(property => property.Name.Contains(key, StringComparison.CurrentCultureIgnoreCase));

            if (property is null)
            {
                throw new ArgumentNullException(nameof(key), "Property with this name doesn't exists");
            }

            return (record) => property.GetValue(record) !.ToString() !.Equals(value);
        }

        public override string Command => "delete";
    }
}
