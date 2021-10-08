using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Decorators
{
    /// <summary>
    /// Decorator that wrap instance of <see cref="IFileCabinetService"/> and add profiling to it.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Interfaces.IFileCabinetService" />
    public sealed class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopwatch = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Record service.</param>
        /// <exception cref="System.ArgumentNullException">Throws when record service is null.</exception>
        public ServiceMeter(IFileCabinetService service) => this.service =
            service ?? throw new ArgumentNullException(nameof(service), "Service can't be null");

        /// <inheritdoc/>
        public void AddRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.AddRecord(record);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByDayOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public void EditRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.EditRecord(record);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var result = this.service.GetRecords();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords(IRecordQuery query)
        {
            this.stopwatch.Restart();
            var result = this.service.GetRecords(query);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedMilliseconds);
            return result;
        }

        /// <inheritdoc/>
        public RecordSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var result = this.service.MakeSnapshot();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public int Restore(RecordSnapshot restoreSnapshot)
        {
            this.stopwatch.Restart();
            var result = this.service.Restore(restoreSnapshot);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public (int AliveRecords, int DeletedRecords) GetStat()
        {
            this.stopwatch.Restart();
            var result = this.service.GetStat();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public void DeleteRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.DeleteRecord(record);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public string Purge()
        {
            this.stopwatch.Restart();
            var result = this.service.Purge();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        private static void WriteResult(long ticks, [CallerMemberName] string methodName = default)
        {
            Console.WriteLine($"{methodName} method execution duration is {ticks} ticks.");
        }
    }
}
