using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.V2.Helpers.SimpleStorage
{
    public class SimpleTableHelper
    {
        CloudTable _table;
        ILogger logger;
        public SimpleTableHelper(CloudTable table, ILogger logger)
        {
            this._table = table;
            this.logger = logger;
        }
        public Task Delete(ITableEntity entity)
        {
            return _table.DeleteEntity(entity);
        }

        public async Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, int? TakeCount, params string[] columns) where TEntity : ITableEntity, new()
        {
            logger.LogTrace($"Query {qryString}");
            return await _table.Query<TEntity>(qryString, TakeCount, columns);
        }


        public async Task<IEnumerable<TEntity>> QueryByPartitionKey<TEntity>(string partitionValue, int? TakeCount, params string[] columns) where TEntity : ITableEntity, new()
        {
            var qryString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionValue);
            logger.LogTrace($"Query {qryString}");
            return await _table.Query<TEntity>(qryString, TakeCount, columns);
        }

        public Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, int Take, int Skip, Func<IComparer<TEntity>> Sort, params string[] columns) where TEntity : ITableEntity, new()
        {
            logger.LogTrace($"Query {qryString}");
            return _table.Query<TEntity>(qryString, Take, Skip, Sort, columns);
        }

        public Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, int Take, int Skip, params string[] columns) where TEntity : ITableEntity, new()
        {
            logger.LogTrace($"Query {qryString}");
            return _table.Query<TEntity>(qryString, Take, Skip, columns);
        }

        public Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, params string[] columns) where TEntity : ITableEntity, new()
        {
            logger.LogTrace($"Query {qryString}");
            return _table.Query<TEntity>(qryString, columns);
        }

        public Task Insert(IEnumerable<ITableEntity> entities)
        {
            try
            {
                logger.LogTrace($"IEnumerable SimpleTableHelper.Insert");
                return _table.Insert(entities);
            }
            catch (StorageException ex)
            {
                logger.LogDebug($"{ex.Message} {ex.StackTrace} SimpleTableHelper.Insert");
                throw;
            }
        }

        public Task Insert(ITableEntity entity)
        {

            try
            {
                logger.LogTrace($"SimpleTableHelper.Insert");

                return _table.Insert(entity);
            }
            catch (StorageException ex)
            {
                logger.LogDebug($"{ex.Message} {ex.StackTrace} IEnumerable SimpleTableHelper.Insert");
                throw;
            }
        }

        public Task<ITableEntity> Replace(ITableEntity entity)
        {
            try
            {
                logger.LogTrace($"SimpleTableHelper.Replace");
                return _table.Replace(entity);
            }
            catch (StorageException ex)
            {
                logger.LogDebug($"{ex.Message} {ex.StackTrace} SimpleTableHelper.Replace");
                throw;
            }
        }

        public Task<T> Get<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            try
            {
                logger.LogTrace($"SimpleTableHelper.Get");
                return _table.Get<T>(partitionKey, rowKey);
            }
            catch (StorageException ex)
            {
                logger.LogDebug($"{ex.Message} {ex.StackTrace} SimpleTableHelper.Get");
                throw;
            }
        }
    }
}
