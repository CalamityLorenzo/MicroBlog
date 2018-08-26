using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.V2.Helpers.SimpleStorage
{
    public interface ISimpleTableHelper
    {
        Task DeleteEntity(ITableEntity entity);
        Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string qryString, int Take, int Skip, Func<IComparer<TEntity>> Sort, params string[] columns) where TEntity : ITableEntity, new();
        Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string qryString, int Take, int Skip, params string[] columns) where TEntity : ITableEntity, new();
        Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string qryString, params string[] columns) where TEntity : ITableEntity, new();
        Task<T> Get<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();
        Task Insert(IEnumerable<ITableEntity> entities);
        Task Insert(ITableEntity entity);
        Task<ITableEntity> Replace(ITableEntity entity);
    }
}