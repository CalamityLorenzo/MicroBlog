using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.V2.Helpers.Context
{
    // Basic Access to storage account and all the queues you can muster mister
    public class CloudStorageContext
    {
        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudQueueClient _queueClient;
        private readonly CloudTableClient _tableClient;
        private readonly CloudFileClient _fileClient;
        private readonly ConcurrentDictionary<string, CloudQueue> Queues = new ConcurrentDictionary<string, CloudQueue>();
        private readonly ConcurrentDictionary<string, CloudBlobContainer> BlobContainers = new ConcurrentDictionary<string, CloudBlobContainer>();
        private readonly ConcurrentDictionary<string, CloudTable> Tables = new ConcurrentDictionary<string, CloudTable>();
        private readonly ConcurrentDictionary<string, CloudFileShare> FileShares = new ConcurrentDictionary<string, CloudFileShare>();

        public SimpleQueueHelper CreateQueueHelper(string tagQueueName)
        {
            return new SimpleQueueHelper(this, tagQueueName);
        }

        public SimpleTableHelper CreateTableHelper(string tableName)
        {
            return new SimpleTableHelper(this, tableName);
        }

        public SimpleBlobHelper CreateBlobHelper(string tableName)
        {
            return new SimpleBlobHelper(this, tableName);
        }

        public CloudStorageContext(string storageAccount)
        {
            _account = CloudStorageAccount.Parse(storageAccount);
            _blobClient = _account.CreateCloudBlobClient();
            _queueClient = _account.CreateCloudQueueClient();
            _tableClient = _account.CreateCloudTableClient();
        }

        public CloudStorageContext(string storageAccount, string key)
        {
            _account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(storageAccount, key), true);
            _blobClient = _account.CreateCloudBlobClient();
            _queueClient = _account.CreateCloudQueueClient();
            _tableClient = _account.CreateCloudTableClient();
            //_fileClient = this._account.CreateCloudFileClient();
        }

        public async Task<CloudQueue> GetQueue(string QueueName, bool CreateIfNotExists = true)
        {
            var q = Queues.GetOrAdd(QueueName, _queueClient.GetQueueReference(QueueName));
            if (CreateIfNotExists)
            {
                await q.CreateIfNotExistsAsync();
            }

            return q;
        }

        public async Task<CloudTable> GetTable(string Tablename, bool CreateIfNotExists = true)
        {
            var t = Tables.GetOrAdd(Tablename, _tableClient.GetTableReference(Tablename));
            if (CreateIfNotExists)
            {
                await t.CreateIfNotExistsAsync();
            }

            return t;
        }

        public async Task<CloudBlobContainer> GetBlobContainer(string container, bool CreateIfNotExists = true)
        {
            var bc = BlobContainers.GetOrAdd(container, _blobClient.GetContainerReference(container));
            if (CreateIfNotExists)
            {
                await bc.CreateIfNotExistsAsync();
            }

            return bc;
        }

        public async Task<CloudFileShare> GetFileShare(string fileShare)
        {
            var fs = FileShares.GetOrAdd(fileShare, _fileClient.GetShareReference(fileShare));
            await fs.CreateIfNotExistsAsync();
            return fs;
        }

        public class SimpleBlobHelper
        {
            private CloudStorageContext CsCtx { get; }
            private string BlobContainer { get; }

            public SimpleBlobHelper(CloudStorageContext csCtx, string blobStoreName)
            {
                CsCtx = csCtx;
                BlobContainer = blobStoreName;
            }

            public async Task AddNewJsonFile(string entity, string fileName)
            {
                var blobStore = await CsCtx.GetBlobContainer(BlobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                await file.UploadTextAsync(entity);
            }

            public async Task AddNewStreamFile(Stream entity, string fileName)
            {
                var blobStore = await CsCtx.GetBlobContainer(BlobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                await file.UploadFromStreamAsync(entity);
            }

            public async Task DeleteBlob(string fileName)
            {
                var blobStore = await CsCtx.GetBlobContainer(BlobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                await file.DeleteIfExistsAsync();
            }

            public async Task<string> GetJsonBlob(string fileName)
            {
                var blobStore = await CsCtx.GetBlobContainer(BlobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                if (await file.ExistsAsync())
                {
                    return await file.DownloadTextAsync();
                }

                return string.Empty;
            }

        }

        public class SimpleTableHelper
        {
            public CloudStorageContext cscCtx;
            public string TableName;
            public SimpleTableHelper(CloudStorageContext csc, string tableName)
            {
                cscCtx = csc;
                TableName = tableName;
            }

            public async Task InsertToTable(ITableEntity entity)
            {
                var table = await cscCtx.GetTable(TableName, true);
                var addItem = TableOperation.Insert(entity);
                await table.ExecuteAsync(addItem);
            }

            // same partionKey applies
            public async Task InsertBulkToTable(IEnumerable<ITableEntity> entity)
            {
                var table = await cscCtx.GetTable(TableName, true);
                var batchOp = new TableBatchOperation();

            }

            public async Task DeleteEntity(ITableEntity entity)
            {
                var table = await cscCtx.GetTable(TableName, true);
                var addItem = TableOperation.Delete(entity);
                await table.ExecuteAsync(addItem);
            }

            public async Task ReplaceEntity(ITableEntity entity)
            {
                var table = await cscCtx.GetTable(TableName, true);
                var replaceItem = TableOperation.Replace(entity);
                await table.ExecuteAsync(replaceItem);
            }

            public async Task<T> GetEntity<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
            {
                var table = await cscCtx.GetTable(TableName, true);
                var entity = await table.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
                return entity.Result as T;
            }

            public async Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string qryString, params string[] columns) where TEntity : ITableEntity, new()
            {
                var table = await cscCtx.GetTable(TableName);
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

            public Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string qryString, int Take, int Skip, params string[] columns) where TEntity : ITableEntity, new()
            {
                return this.EntityQuery<TEntity>(qryString, Take, Skip, null, columns);
            }

            public async Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string qryString, int Take, int Skip, Func<IComparer<TEntity>> Sort, params string[] columns) where TEntity : ITableEntity, new()
            {
                var table = await cscCtx.GetTable(TableName);
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

        public class SimpleQueueHelper
        {
            private CloudStorageContext cscCtx;
            private string QueueName;
            public SimpleQueueHelper(CloudStorageContext csc, string queueName)
            {
                cscCtx = csc;
                QueueName = queueName;
            }

            public async Task InsertIntoQueue(string serialisedMessage)
            {
                var queue = await cscCtx.GetQueue(QueueName);
                await queue.AddMessageAsync(new CloudQueueMessage(serialisedMessage));
            }
        }
    }
}