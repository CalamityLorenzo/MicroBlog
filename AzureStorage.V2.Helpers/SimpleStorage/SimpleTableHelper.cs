using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.V2.Helpers.SimpleStorage
{
   public class SimpleTableHelper
    {
        CloudTable _table;
        public SimpleTableHelper(CloudTable table)
        {
            this._table = table;
        }
        public Task Delete(ITableEntity entity)
        {
            return _table.DeleteEntity(entity);
        }

        public async Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, int? TakeCount, params string[] columns) where TEntity : ITableEntity, new()
        {
            return await _table.Query<TEntity>(qryString, TakeCount, columns);
        }


        public Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, int Take, int Skip, Func<IComparer<TEntity>> Sort, params string[] columns) where TEntity : ITableEntity, new()
        {
            return _table.Query<TEntity>(qryString, Take, Skip, Sort, columns);
        }

        public Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, int Take, int Skip, params string[] columns) where TEntity : ITableEntity, new()
        {
            return _table.Query<TEntity>(qryString, Take, Skip, columns);
        }

        public Task<IEnumerable<TEntity>> Query<TEntity>(string qryString, params string[] columns) where TEntity : ITableEntity, new()
        {
            return _table.Query<TEntity>(qryString,  columns);
        }

        public Task Insert(IEnumerable<ITableEntity> entities)
        {
            return _table.Insert(entities);
        }

        public Task Insert(ITableEntity entity)
        {
            return _table.Insert(entity);
        }

        public Task<ITableEntity> Replace(ITableEntity entity)
        {
            return _table.Replace(entity);
        }

        public Task<T> Get<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            return _table.Get<T>(partitionKey, rowKey);
        }
    }
}
