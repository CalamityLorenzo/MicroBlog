
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using MicroBlog.V3.Functions.Models;
using MicroBlog.V3.Services;
using System.Threading.Tasks;
using MicroBlog.V3.Functions.Models.App;
using MicroBlog.V3.Functions.Settings;
using AzureStorage.V2.Helpers.Context;
using Microsoft.Extensions.Logging;

namespace MicroBlog.V3.Functions
{
    public static class InsertBlogEntry
    {
        [FunctionName("InsertBlogEntry")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("CreateBlogEntry processed a request.");

            var completeArticle = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            var articleInput = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            var mOpts = AppConfigSettings.GetOptions();

            var bas = new BlogArticleService(new CloudStorageContext(mOpts.StorageAccount), mOpts, log);
            var article = await bas.Add(completeArticle);

            return new OkObjectResult(article);
        }
    }
}
