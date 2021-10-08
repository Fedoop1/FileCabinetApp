using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Decorators
{
    public sealed class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopwatch = new ();

        public ServiceMeter(IFileCabinetService service) => this.service =
            service ?? throw new ArgumentNullException(nameof(service), "Service can't be null");

        public void AddRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.AddRecord(record);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
        }

        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByDayOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public void EditRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.EditRecord(record);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
        }

        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var result = this.service.GetRecords();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public IEnumerable<FileCabinetRecord> GetRecords(IRecordQuery query)
        {
            this.stopwatch.Restart();
            var result = this.service.GetRecords(query);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedMilliseconds);
            return result;
        }

        public RecordSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var result = this.service.MakeSnapshot();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public int Restore(RecordSnapshot restoreSnapshot)
        {
            this.stopwatch.Restart();
            var result = this.service.Restore(restoreSnapshot);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public (int AliveRecords, int DeletedRecords) GetStat()
        {
            this.stopwatch.Restart();
            var result = this.service.GetStat();
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
            return result;
        }

        public void DeleteRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.DeleteRecord(record);
            this.stopwatch.Stop();
            WriteResult(this.stopwatch.ElapsedTicks);
        }

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
