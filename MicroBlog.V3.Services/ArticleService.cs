using AzureStorage.V2.Helpers;
using AzureStorage.V2.Helpers.Context;
using AzureStorage.V2.Helpers.SimpleStorage;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using MicroBlog.V3.Services.Models;
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
        private LazyAsync<SimpleBlobHelper> articleBlobStorage;
        private LazyAsync<SimpleTableHelper> articleDetailsStorage;

        internal ArticleService(CloudStorageContext cscCtx, MicroBlogOptions opts)
        {
            this.cscCtx = cscCtx;
            this.opts = opts;
            articleBlobStorage = new LazyAsync<SimpleBlobHelper>(async ()=> await cscCtx.CreateBlobHelper(opts[StorageList.ArticleBlob]));
            articleDetailsStorage = new LazyAsync<SimpleTableHelper>(async () => await cscCtx.CreateTableHelper(opts[StorageList.ArticleDetails]));
        }


        public async Task<IClientArticle> GetByUrl(string url)
        {
            var articleDetails = await articleDetailsStorage.Value;
            var qry = TableQuery.GenerateFilterCondition("Url", QueryComparisons.Equal, url);
            var results = (await articleDetails.EntityQuery<ArticleDetails>(qry)).ToList();
            if (results.Count > 0)
            {
                var details = results.First();
                var jsonBlob = await (await articleBlobStorage.Value).GetJsonBlob($"{details.Id}.json");
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
                foreach (var ex in excep.InnerExceptions)
                {
                    Console.Write(ex.Message + " " + ex.StackTrace);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return null;
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
            var articleBlobStore= await articleBlobStorage.Value;

            await articleDetailsTable.Insert(articleDetails);
            await articleBlobStore.AddNewJsonFile(articleBlobString, $"{Id}.json");

            return new CompleteArticle(article, Id);
        }

        public async Task<IClientArticle> Get(Guid Id)
        {
            var articleJsonBlob = await articleBlobStorage.Value;
            var jsonBlob = await articleJsonBlob.GetJsonBlob($"{Id}.json");
            var article = JsonConvert.DeserializeObject<ArticleFileData>(jsonBlob);
            var articleTables = await this.articleDetailsStorage.Value;
            var details = await articleTables.Get<ArticleDetails>(Id.ToString(), ArticleDetails.RowKeyDef);
            return new CompleteArticle(article, details);
        }

        public async Task Delete(IClientArticle article)
        {
            await this.Delete(article.Id);
        }

        public async Task Delete(Guid Id)
        {

            var articleTables = await this.articleDetailsStorage.Value;
            var articleBlobStore = await this.articleBlobStorage.Value;
            var details = await articleTables.Get<ArticleDetails>(Id.ToString(), ArticleDetails.RowKeyDef);
            await articleTables.Delete(details);
            await articleBlobStore.DeleteBlob($"{Id}.json");
        }

        public async Task<IClientArticle> Update(IClientArticle article)
        {

            // Update is actually a delete then re-insert
            // Maintaining the Id
            var articleTables = await this.articleDetailsStorage.Value;
            await articleTables.Delete(new ArticleDetails(article));
            return await this.InsertArticle(article, article.Id);

        }

        public async Task<IEnumerable<IArticleDetails>> FindArticlDetails(DateTime start, DateTime end, int take, int skip)
        {
            var qry = $"(Published ge datetime'{start.Date}') and (Published le datetime'{end.Date}')";
            var articleTables = await this.articleDetailsStorage.Value;
            return await articleTables.EntityQuery<ArticleDetails>(qry, take, skip);
        }

        public static IArticleService GetManager()
        {
            var opts = MicroBlogConfiguration.GetOptions();
            return new ArticleService(new CloudStorageContext(opts.StorageAccount), opts);
        }
    }
}
