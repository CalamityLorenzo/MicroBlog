using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage.V2.Helpers
{
   public  static class TableExtensions
    {

        public static async Task Insert(this CloudTable table, ITableEntity entity)
        {
            var addItem = TableOperation.Insert(entity);
            await table.ExecuteAsync(addItem);
        }

        // same partionKey applies
        public static async Task Insert(this CloudTable table, IEnumerable<ITableEntity> entities)
        {
            // Shove them all in nom nom nom
            var manageableRecords = entities.ToList().SplitList(100);

            // Now make create the Table operation
            List<TableBatchOperation> tableOps = new List<TableBatchOperation>();

            var counter = 0;
            foreach (var bagOThings in manageableRecords)
            {
                var Insert = new TableBatchOperation();
                bagOThings.ForEach(o => { o.PartitionKey = o.PartitionKey + counter.ToString().PadLeft(5, '0'); Insert.Insert(o); });
                tableOps.Add(Insert);
                counter += 1;
            }

            // var tasks =  manageableBatch.Select(table.ExecuteBatchAsync).ToList();
            var tasks = tableOps.Select(table.ExecuteBatchAsync);
            //Task.WaitAll(tasks.ToArray());

            await Task.WhenAll(tasks);
        }

        public static  async Task DeleteEntity(this CloudTable table, ITableEntity entity)
        {
            var addItem = TableOperation.Delete(entity);
            await table.ExecuteAsync(addItem);
        }

        public static async Task<ITableEntity> Replace(this CloudTable table, ITableEntity entity)
        {
            var replaceItem = TableOperation.Replace(entity);
            return (await table.ExecuteAsync(replaceItem)).Result as ITableEntity;
        }

        public static async Task<T> Get<T>(this CloudTable table, string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            var entity = await table.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
            return entity.Result as T;
        }

        public static async Task<IEnumerable<TEntity>> EntityQuery<TEntity>(this CloudTable table, string qryString, params string[] columns) where TEntity : ITableEntity, new()
        {
            
            var tQuery = new TableQuery<TEntity>() { FilterString = qryString, SelectColumns = columns };
            var token = new TableContinuationToken();
            var returnResults = new List<TEntity>();
            do
            {
                var qryRes = await table.ExecuteQuerySegmentedAsync<TEntity>(tQuery, token);
                token = qryRes.ContinuationToken;
                returnResults.AddRange(qryRes.Results);
            } while (token != null);
            return returnResults;
        }

        public static Task<IEnumerable<TEntity>> EntityQuery<TEntity>(this CloudTable table, string qryString, int Take, int Skip, params string[] columns) where TEntity : ITableEntity, new()
        {
            return TableExtensions.EntityQuery<TEntity>(table, qryString, Take, Skip, null, columns);
        }

        public static async Task<IEnumerable<TEntity>> EntityQuery<TEntity>(this CloudTable table, string qryString, int Take, int Skip, Func<IComparer<TEntity>> Sort, params string[] columns) where TEntity : ITableEntity, new()
        {
            var tQuery = new TableQuery<TEntity>() { FilterString = qryString, SelectColumns = columns };

            var token = new TableContinuationToken();
            var returnResults = new List<TEntity>();
            do
            {
                var qryRes = await table.ExecuteQuerySegmentedAsync<TEntity>(tQuery, token);
                token = qryRes.ContinuationToken;
                returnResults.AddRange(qryRes.Results);

            } while (token != null && returnResults.Count <= Take + Skip);
            // Cheap
            if (Sort != null)
            {
                returnResults.Sort(Sort());
            }

            return returnResults.Skip(Skip).Take(Take);
        }

    }
}
