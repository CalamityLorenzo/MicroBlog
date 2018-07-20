using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroBlog.V3.Services.Context
{
    public class wwwSimpleTableHelper
    {
        public CloudStorageContext cscCtx;
        public string TableName;
        public wwwSimpleTableHelper(CloudStorageContext csc, string tableName)
        {
            this.cscCtx = csc;
            TableName = tableName;
        }

        private async Task InsertToTable(ITableEntity entity)
        {
            CloudTable table = await cscCtx.GetTable(TableName, true);
            TableOperation addItem = TableOperation.Insert(entity);
            await table.ExecuteAsync(addItem);
        }

        private async Task DeleteEntity(ITableEntity entity)
        {
            CloudTable table = await cscCtx.GetTable(TableName, true);
            TableOperation addItem = TableOperation.Delete(entity);
            await table.ExecuteAsync(addItem);
        }

        private async Task ReplaceEntity(ITableEntity entity)
        {
            CloudTable table = await cscCtx.GetTable(TableName, true);
            TableOperation replaceItem = TableOperation.Replace(entity);
            await table.ExecuteAsync(replaceItem);
        }

        private async Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string tableName, string qry, List<String> columns) where TEntity : ITableEntity, new()
        {
            var table = await cscCtx.GetTable(tableName);
            var tQuery = new TableQuery<TEntity>() { FilterString = qry, SelectColumns = columns };
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
        
    }
}
