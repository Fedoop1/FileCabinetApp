﻿using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.DataTransfer;
using FileCabinetApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileCabinetApp.Decorators
{
    /// <summary>
    /// Class decorator which wrap instance of <see cref="IFileCabinetService"/> and log all information about it activity.
    /// </summary>
    /// <seealso cref="IFileCabinetService" />
    public sealed class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when service is null
        /// or
        /// Throws when logger is null.
        /// </exception>
        public ServiceLogger(IFileCabinetService service, ILogger<ServiceLogger> logger) =>
            (this.service, this.logger) = (
                service ?? throw new ArgumentNullException(nameof(service), "Service can't be null"),
                logger ?? throw new ArgumentNullException(nameof(logger), "Logger can't be null"));

        /// <inheritdoc/>
        public IRecordValidator CreateValidator()
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling CreateValidator()");
            var result = this.service.CreateValidator();
            this.logger.LogInformation($"{DateTime.Now} - CreateValidator() return instance of {result.GetType()}");
            return result;
        }

        /// <inheritdoc/>
        public int CreateRecord()
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling CreateRecord()");
            var result = this.service.CreateRecord();
            this.logger.LogInformation($"{DateTime.Now} - CreateRecord() create record with id = {result}");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling FindByFirstName(firstName) with firstName = {firstName}");
            var result = this.service.FindByFirstName(firstName);
            this.logger.LogInformation($"{DateTime.Now} - FindByFirstName(firstName) return {result.Count()} record(s)");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling FindByLastName(lastName) with lastName = {lastName}");
            var result = this.service.FindByLastName(lastName);
            this.logger.LogInformation($"{DateTime.Now} - FindByLastName(lastName) return {result.Count()} record(s)");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDayOfBirth(string dateOfBirth)
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling FindByDayOfBirth(dateOfBirth) with dateOfBirth = {dateOfBirth}");
            var result = this.service.FindByDayOfBirth(dateOfBirth);
            this.logger.LogInformation($"{DateTime.Now} - FindByDayOfBirth(dateOfBirth) return {result.Count()} record(s)");
            return result;
        }

        /// <inheritdoc/>
        public bool EditRecord(int id)
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling EditRecord(id) with id = {id}");
            var result = this.service.EditRecord(id);
            this.logger.LogInformation($"{DateTime.Now} - EditRecord(id) finished it work with result: {result}");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling GetRecords()");
            var result = this.service.GetRecords();
            this.logger.LogInformation($"{DateTime.Now} - GetRecords() return {result.Count()} record(s)");
            return result;
        }

        /// <inheritdoc/>
        public RecordShapshot MakeSnapshot()
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling MakeSnapshot()");
            var result = this.service.MakeSnapshot();
            this.logger.LogInformation($"{DateTime.Now} - MakeSnapshot() return snapshot");
            return result;
        }

        /// <inheritdoc/>
        public int Restore(RecordShapshot snapshot)
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling Restore(restoreSnapshot)");
            var result = this.service.Restore(snapshot);
            this.logger.LogInformation($"{DateTime.Now} - Restore(restoreSnapshot) finished it work with result: {result}");
            return result;
        }

        /// <inheritdoc/>
        public (int AliveRecords, int DeletedRecords) GetStat()
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling GetStat()");
            var result = this.service.GetStat();
            this.logger.LogInformation($"{DateTime.Now} - GetStat() return actual records: {result.AliveRecords}, deleted records {result.DeletedRecords}");
            return result;
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int index)
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling RemoveRecord(int index) with index = {index}");
            var result = this.service.RemoveRecord(index);
            this.logger.LogInformation($"{DateTime.Now} - RemoveRecord(int index) return result: {result}");
            return result;
        }

        /// <inheritdoc/>
        public string Purge()
        {
            this.logger.LogInformation($"{DateTime.Now} - Calling Purge()");
            var result = this.service.Purge();
            this.logger.LogInformation($"{DateTime.Now} - Purge() return : {result}");
            return result;
        }
    }
}
