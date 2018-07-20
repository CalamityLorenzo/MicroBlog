using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroBlog.V3.Services.Context
{
    public partial class MicroBlogContext : CloudStorageContext
    {
        //private string newArticleQueue, articleDetails, articleStore, tagStore, tagProcessorQ;

        private string _newArticleBlobStorage, _tagStorageTable, _tagProcessorQ, _articleDetailsTable;

        //public MicroBlogContext(string storageAccount, string key, string newArticleQueue, string articleDetails, string articleStore, string tagStore, string tagProcessorQ) : base(storageAccount, key)
        //{
        //    this.newArticleQueue = newArticleQueue;
        //    this.articleDetails = articleDetails;
        //    this.tagStore = tagStore;
        //    this.tagProcessorQ = tagProcessorQ;
        //}

        //public MicroBlogContext(string storageAccount, string newArticleQueue, string articleDetails, string articleStore, string tagStore, string tagProcessorQ) : base(storageAccount)
        //{
        //    this.newArticleQueue = newArticleQueue;
        //    this.articleDetails = articleDetails;
        //    this.tagStore = tagStore;
        //    this.tagProcessorQ = tagProcessorQ;
        //}

        internal MicroBlogContext(string storageAccountDetails, string key, string newArticleBlobStorage, string articleDetails, string tagsProcessorQ, string tagsTable) : base(storageAccountDetails, key)
        {
            _newArticleBlobStorage = newArticleBlobStorage;
            _tagStorageTable = tagsTable;
            _articleDetailsTable = articleDetails;
            _tagProcessorQ = tagsProcessorQ;

        }

        internal MicroBlogContext(string storageAccountDetails, string newArticleBlobStorage, string articleDetails, string tagsProcessorQ, string tagsTable) : base(storageAccountDetails)
        {
            _newArticleBlobStorage = newArticleBlobStorage;
            _tagStorageTable = tagsTable;
            _articleDetailsTable = articleDetails;
            _tagProcessorQ = tagsProcessorQ;

        }

        internal async Task<ArticleDetails> GetArticleDetails(Guid id)
        {
            CloudTable ArticleDetails = await GetTable(_articleDetailsTable);
            TableOperation getIt = TableOperation.Retrieve<ArticleDetails>(id.ToString(), "0");

            TableResult itm = await ArticleDetails.ExecuteAsync(getIt);
            return itm.Result as ArticleDetails;
        }

        internal async Task DeleteArticleFile(Guid id)
        {
            ArticleDetails TableEnt = await GetArticleDetails(id);
            CloudTable ArticleDetails = await GetTable(_articleDetailsTable);
            TableOperation deleteIt = TableOperation.Delete(TableEnt);
            await ArticleDetails.ExecuteAsync(deleteIt);
        }

        internal async Task<IArticleTags> GetArticleTags(Guid entityId)
        {
            CloudTable table = await GetTable(_tagStorageTable);
            TableResult tagsResult = await table.ExecuteAsync(TableOperation.Retrieve<ArticleTags>("tags", entityId.ToString()));
            return (tagsResult.Result != null) ? tagsResult.Result as ArticleTags : ArticleTags.Empty();
        }

        internal async Task RemoveArticleDetails(Guid id)
        {
            ArticleDetails artDeets = await GetArticleDetails(id) as ArticleDetails;
            CloudTable table = await GetTable(_articleDetailsTable);
            await table.ExecuteAsync(TableOperation.Delete(artDeets));
        }

        internal async Task<string> GetArticleFileData(string fileName)
        {
            CloudBlobContainer ArticleStorage = await GetBlobContainer(_newArticleBlobStorage);
            await ArticleStorage.CreateIfNotExistsAsync();
            CloudBlockBlob fileData = ArticleStorage.GetBlockBlobReference(fileName);
            if (await fileData.ExistsAsync())
            {
                string fileString = await fileData.DownloadTextAsync();
                return fileString;
            }
            else
            {
                return string.Empty;
            }
        }

        public static MicroBlogContext GetContext()
        {
            MicroBlogConfiguration.Options opts = MicroBlogConfiguration.GetOptions();
            return new MicroBlogContext(opts.StorageAccount, opts.ArticleBlob, opts.ArticleDetails, opts.TagQ, opts.TagsTable);
        }
    }
}
