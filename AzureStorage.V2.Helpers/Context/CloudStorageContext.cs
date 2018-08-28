using AzureStorage.V2.Helpers.SimpleStorage;
using Microsoft.Extensions.Logging;
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

        public async Task<SimpleQueueHelper> CreateQueueHelper(string tagQueueName, ILogger logger)
        {
            return new SimpleQueueHelper(this, tagQueueName, logger);
        }

        public async Task<SimpleTableHelper> CreateTableHelper(string tableName, ILogger logger)
        {
            return new SimpleTableHelper(await this.GetTable(tableName), logger);
        }

        public async Task<SimpleBlobHelper> CreateBlobHelper(string tableName, ILogger logger)
        {
            return new SimpleBlobHelper(this, tableName, logger);
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
     
        #region simpleHelpers
        public class SimpleBlobHelper
        {
            private CloudStorageContext CsCtx { get; }
            private string BlobContainer { get; }

            public SimpleBlobHelper(CloudStorageContext csCtx, string blobStoreName, object logger)
            {
                CsCtx = csCtx;
                BlobContainer = blobStoreName;
            }

            public async Task AddNewStringFile(string entity, string fileName)
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

            public async Task<string> GetStringBlob(string fileName)
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

        public class SimpleQueueHelper
        {
            private CloudStorageContext cscCtx;
            private readonly string QueueName;
            public SimpleQueueHelper(CloudStorageContext csc, string queueName, ILogger logger)
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
        #endregion
    }
}