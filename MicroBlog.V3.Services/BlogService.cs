using AzureStorage.V2.Helpers.Context;
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

namespace MicroBlog.V3.Services
{
    public class BlogService : IBlogService
    {
        private CloudStorageContext cscCtx;

        private SimpleBlobHelper articleBlobStorage;
        private SimpleTableHelper articleDetailsStorage;

        internal BlogService(CloudStorageContext cscCtx, Options opts)
        {
            articleBlobStorage = cscCtx.CreateBlobHelper(opts.ArticleBlob);
            articleDetailsStorage = cscCtx.CreateTableHelper(opts.ArticleDetails);
            this.cscCtx = cscCtx;
        }

        public async Task<IClientArticle> GetByUrl(string url)
        {
            var qry = TableQuery.GenerateFilterCondition("Url", QueryComparisons.Equal, url);
            var results = (await articleDetailsStorage.EntityQuery<ArticleDetails>(qry)).ToList();
            if (results.Count > 0)
            {
                var details = results.First();
                var jsonBlob = await articleBlobStorage.GetJsonBlob($"{details.Id}.json");
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
            await articleDetailsStorage.InsertToTable(articleDetails);
            await  articleBlobStorage.AddNewJsonFile (articleBlobString, $"{Id}.json");
          
            return new CompleteArticle(article, Id);
        }

        public async Task<IClientArticle> Get(Guid Id)
        {
            var jsonBlob = await articleBlobStorage.GetJsonBlob($"{Id}.json");
            var article = JsonConvert.DeserializeObject<ArticleFileData>(jsonBlob);
            var details = await this.articleDetailsStorage.GetEntity<ArticleDetails>(Id.ToString(), ArticleDetails.RowKeyDef);
            return new CompleteArticle(article, details);
        }

        public async Task Delete(IClientArticle article)
        {
            await this.Delete(article.Id);
        }

        public async Task Delete(Guid Id)
        {

            var details = await this.articleDetailsStorage.GetEntity<ArticleDetails>(Id.ToString(), ArticleDetails.RowKeyDef);
            await this.articleDetailsStorage.DeleteEntity(details);
            await this.articleBlobStorage.DeleteBlob($"{Id}.json");
        }

        public async Task<IClientArticle> Update(IClientArticle article)
        {

            // Update is actually a delete then re-insert
            // Maintaining the Id
            await this.Delete(article.Id);
            return await this.InsertArticle(article, article.Id);

        }

        public async Task<IEnumerable<IArticleDetails>> FindAllDetails(DateTime start, DateTime end, int take, int skip)
        {
            return null;
        }


        public static IBlogService GetManager()
        {
            var opts = MicroBlogConfiguration.GetOptions();
            return new BlogService(new CloudStorageContext(opts.StorageAccount), opts );
        }
    }
}
