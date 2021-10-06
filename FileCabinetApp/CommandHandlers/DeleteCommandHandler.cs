using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.CommandHandlers.CommandHandlerExtensions;

#pragma warning disable CA1308 // Normalize strings to uppercase
#pragma warning disable CA1031 // Do not catch general exception types

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle "Delete" command to import data from file in special format.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
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
        public override string Command => "delete";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!string.IsNullOrEmpty(commandRequest?.Command) && commandRequest.Command.Contains("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                this.Delete(commandRequest.Parameters);
                return;
            }

            this.NextHandle?.Handle(commandRequest);
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

                var parametersArray = parameters.ToLowerInvariant().Split(
                    new[] { "where" },
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parametersArray.Length != ParametersCount)
                {
                    Console.WriteLine("Invalid parameters count");
                    return;
                }

                var pair = ExtractKeyValuePair(parametersArray[0], new[] { "=" });
                var predicate = GeneratePredicate(pair);

                List<int> deletedRecordsId = new ();
                foreach (var record in this.Service.GetRecords(new RecordQuery(predicate, GenerateHashCode(pair))))
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
    }
}
