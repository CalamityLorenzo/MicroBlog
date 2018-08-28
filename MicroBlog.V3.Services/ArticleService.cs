using AzureStorage.V2.Helpers;
using AzureStorage.V2.Helpers.Context;
using AzureStorage.V2.Helpers.SimpleStorage;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using MicroBlog.V3.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AzureStorage.V2.Helpers.Context.CloudStorageContext;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration.MicroBlogOptions;

namespace MicroBlog.V3.Services
{
    public class ArticleService : IArticleService
    {
        private readonly CloudStorageContext cscCtx;
        private readonly MicroBlogOptions opts;
        private readonly ILogger logger;
        private LazyAsync<SimpleBlobHelper> articleBlobStorage;
        private LazyAsync<SimpleTableHelper> articleDetailsStorage;

        internal ArticleService(CloudStorageContext cscCtx, MicroBlogOptions opts, ILogger logger)
        {
            this.cscCtx = cscCtx;
            this.opts = opts;
            this.logger = logger;
            articleBlobStorage = new LazyAsync<SimpleBlobHelper>(async () => await cscCtx.CreateBlobHelper(opts[StorageList.ArticleBlob], logger));
            articleDetailsStorage = new LazyAsync<SimpleTableHelper>(async () => await cscCtx.CreateTableHelper(opts[StorageList.ArticleDetails], logger));
        }


        public async Task<IClientArticle> GetByUrl(string url)
        {
            var articleDetails = await articleDetailsStorage.Value;
            var qry = TableQuery.GenerateFilterCondition("Url", QueryComparisons.Equal, url);
            var urlKey = (await articleDetails.Query<ArticleDetailsUrlId>(qry, 1)).ToList();
            if (urlKey.Count > 0)
            {
                var urlData = urlKey.First();
                var details = await articleDetails.Get<ArticleDetails>(urlData.Id, urlData.Url);
                var jsonBlob = await (await articleBlobStorage.Value).GetStringBlob($"{details.Id}.json");
                var article = JsonConvert.DeserializeObject<ArticleFileData>(jsonBlob);
                return new CompleteArticle(article, details);

            }
            else
            {
                return CompleteArticle.Empty();
            }
        }

        public async Task<IClientArticle> Create(IClientArticle article)
        {
            try
            {
                return await this.InsertArticle(article, Guid.NewGuid());
            }
            catch (AggregateException excep)
            {
                logger.LogCritical("Aggregate exception");
                foreach (var ex in excep.InnerExceptions)
                {
                    logger.LogDebug(ex.Message + " " + ex.StackTrace);
                }
                throw; 
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex.Message + " " + ex.StackTrace);
                throw;
            }
        }

        private async Task<IClientArticle> InsertArticle(IClientArticle article, Guid Id)
        {
            // Turn the basic article into a Json blob
            var articleData = new ArticleFileData(article, Id);
            // Then store the info in table
            var articleDetails = new ArticleDetails(article, Id);
            var articleBlobString = JsonConvert.SerializeObject(articleData);

            var articleDetailsTable = await articleDetailsStorage.Value;
            var articleBlobStore = await articleBlobStorage.Value;
            await articleDetailsTable.Insert(articleDetails);
            await articleDetailsTable.Insert(new ArticleDetailsUrlId(articleDetails.Url, articleDetails.Id));
            await articleBlobStore.AddNewStringFile(articleBlobString, $"{Id}.json");

            return new CompleteArticle(article, Id);
        }

        public async Task<IClientArticle> Get(Guid Id)
        {
            var articleJsonBlob = await articleBlobStorage.Value;
            var jsonBlob = await articleJsonBlob.GetStringBlob($"{Id}.json");
            var article = JsonConvert.DeserializeObject<ArticleFileData>(jsonBlob);
            var articleTables = await this.articleDetailsStorage.Value;

            var details = await articleTables.Query<ArticleDetails>(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Id.ToString()), 1);
            return new CompleteArticle(article, details.FirstOrDefault());
        }

        public async Task Delete(IClientArticle article)
        {
            await this.Delete(article.Id);
        }

        public async Task Delete(Guid Id)
        {
            var articleTables = await this.articleDetailsStorage.Value;
            var articleBlobStore = await this.articleBlobStorage.Value;
            var details = await articleTables.Query<ArticleDetails>(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Id.ToString()), 1);
            var articleDetails = details.FirstOrDefault();
            var urlKey = await articleTables.Get<ArticleDetailsUrlId>(articleDetails.Url, articleDetails.Id.ToString());
            // TODO: Schism this into one takss
            await articleTables.Delete(articleDetails);
            await articleTables.Delete(urlKey);
            await articleBlobStore.DeleteBlob($"{Id}.json");
        }

        public async Task<IClientArticle> Update(IClientArticle article)
        {
            try
            {
                this.logger.LogTrace($"Updating article {article.Id}");
                // confirm we have not changed the url
                var articleTables = await this.articleDetailsStorage.Value;
                var articleKeys = await articleTables.Get<ArticleDetailsUrlId>(article.Url, article.Id.ToString());
                // we have not changed the url
                if (articleKeys != null)
                {
                    this.logger.LogTrace($"Keys are the same {article.Id}");
                    await articleTables.Replace(new ArticleDetails(article) { ETag = "*" });
                    return article;
                }
                else
                {
                    // we have changed the url (Which is part of the key)
                    // so we have to recreate the whole thing
                    this.logger.LogTrace($"Replaced keys recreating article{article.Id}");
                    await this.Delete(article.Id);
                    return await this.InsertArticle(article, article.Id);
                }
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    logger.LogError(ex.Message);
                }
                throw;
            }
        }

        public async Task<IEnumerable<IArticleDetails>> FindArticlDetails(DateTime start, DateTime end, int take, int skip)
        {
            var qry = $"(Published ge datetime'{start.Date}') and (Published le datetime'{end.Date}')";
            var articleTables = await this.articleDetailsStorage.Value;
            return await articleTables.Query<ArticleDetails>(qry, take, skip);
        }

    }
}
