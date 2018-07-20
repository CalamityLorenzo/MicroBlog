using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace MicroBlog.V3.Services.Context
{
    // Basic Access to storage account and all the queues you can muster mister
    public class CloudStorageContext
    {
        CloudStorageAccount _account;
        CloudBlobClient _blobClient;
        CloudQueueClient _queueClient;
        CloudTableClient _tableClient;
        CloudFileClient _fileClient;
        ConcurrentDictionary<string, CloudQueue> Queues = new ConcurrentDictionary<string, CloudQueue>();
        ConcurrentDictionary<string, CloudBlobContainer> BlobContainers = new ConcurrentDictionary<string, CloudBlobContainer>();
        ConcurrentDictionary<string, CloudTable> Tables = new ConcurrentDictionary<string, CloudTable>();
        ConcurrentDictionary<string, CloudFileShare> FileShares = new ConcurrentDictionary<string, CloudFileShare>();


        public SimpleTableHelper CreateTableHelper(string tableName)
        {
            return new SimpleTableHelper(this, tableName);
        }

        public SimpleBlobHelper CreateBloblHelper(string tableName)
        {
            return new SimpleBlobHelper(this, tableName);
        }


        public CloudStorageContext(string storageAccount)
        {
            this._account = CloudStorageAccount.Parse(storageAccount);
            Init();
        }

        public CloudStorageContext(string storageAccount, string key)
        {
            this._account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("xpclblogstorage", key), true);
            Init();
            //_fileClient = this._account.CreateCloudFileClient();
        }

        private void Init()
        {
            _blobClient = this._account.CreateCloudBlobClient();
            _queueClient = this._account.CreateCloudQueueClient();
            _tableClient = this._account.CreateCloudTableClient();
        }

        public async Task<CloudQueue> GetQueue(string QueueName, bool CreateIfNotExists = true)
        {
            var q = this.Queues.GetOrAdd(QueueName, _queueClient.GetQueueReference(QueueName));
            if (CreateIfNotExists) await q.CreateIfNotExistsAsync();
            return q;
        }

        public async Task<CloudTable> GetTable(string Tablename, bool CreateIfNotExists = true)
        {
            var t = this.Tables.GetOrAdd(Tablename, _tableClient.GetTableReference(Tablename));
            if (CreateIfNotExists) await t.CreateIfNotExistsAsync();
            return t;
        }

        public async Task<CloudBlobContainer> GetBlobContainer(string container, bool CreateIfNotExists = true)
        {
            var bc = this.BlobContainers.GetOrAdd(container, _blobClient.GetContainerReference(container));
            if (CreateIfNotExists) await bc.CreateIfNotExistsAsync();
            return bc;
        }

        public async Task<CloudFileShare> GetFileShare(string fileShare)
        {
            var fs = this.FileShares.GetOrAdd(fileShare, _fileClient.GetShareReference(fileShare));
            await fs.CreateIfNotExistsAsync();
            return fs;
        }

        public class SimpleBlobHelper
        {
            private CloudStorageContext CsCtx { get; }
            private string blobContainer { get; }

            public SimpleBlobHelper(CloudStorageContext csCtx, string blobStoreName)
            {
                CsCtx = csCtx;
                blobContainer = blobStoreName;
            }

            public async Task AddNewJsonFile(string entity, string fileName)
            {
                var blobStore  = await  this.CsCtx.GetBlobContainer(blobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                await file.UploadTextAsync(entity);
            }

            public async Task AddNewStreamFile(Stream entity, string fileName)
            {
                var blobStore = await this.CsCtx.GetBlobContainer(blobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                await file.UploadFromStreamAsync(entity);
            }

            public async Task DeleteBlob(string fileName)
            {
                var blobStore = await this.CsCtx.GetBlobContainer(blobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                await file.DeleteIfExistsAsync();
            }

            public async Task<string> GetJsonFile(string fileName)
            {
                var blobStore = await this.CsCtx.GetBlobContainer(blobContainer);
                var file = blobStore.GetBlockBlobReference(fileName);
                if(await file.ExistsAsync())
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
                this.cscCtx = csc;
                TableName = tableName;
            }

            public async Task InsertToTable(ITableEntity entity)
            {
                CloudTable table = await cscCtx.GetTable(TableName, true);
                TableOperation addItem = TableOperation.Insert(entity);
                await table.ExecuteAsync(addItem);
            }

            public async Task DeleteEntity(ITableEntity entity)
            {
                CloudTable table = await cscCtx.GetTable(TableName, true);
                TableOperation addItem = TableOperation.Delete(entity);
                await table.ExecuteAsync(addItem);
            }


            public async Task ReplaceEntity(ITableEntity entity)
            {
                CloudTable table = await cscCtx.GetTable(TableName, true);
                TableOperation replaceItem = TableOperation.Replace(entity);
                await table.ExecuteAsync(replaceItem);
            }

            public async Task<T>GetEntity<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
            {
                CloudTable table = await cscCtx.GetTable(TableName, true);
                var entity = await table.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
                return entity.Result as T;
            }

            public async Task<IEnumerable<TEntity>> EntityQuery<TEntity>(string tableName, string qry, params string[] columns) where TEntity : ITableEntity, new()
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
}