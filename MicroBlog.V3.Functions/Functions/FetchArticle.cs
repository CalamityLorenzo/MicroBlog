
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using MicroBlog.V3.Functions.Models;
using System.Threading.Tasks;
using MicroBlog.V3.Services;
using MicroBlog.V3.Functions.Models.App;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Functions.Settings;
using Microsoft.Extensions.Logging;
using AzureStorage.V2.Helpers.Context;

namespace MicroBlog.V3.Functions
{
    public static class FetchArticle
    {
        [FunctionName("FetchArticle")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("CreateBlogEntry processed a request.");
            
            var articleInput = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            var mOpts = AppConfigSettings.GetOptions();

            var bas = new BlogArticleService(new CloudStorageContext(mOpts.StorageAccount), mOpts, log);
            
            var article = (articleInput.Url.Length == 0) ? await bas.Get(articleInput.Id) : await bas.Get(articleInput.Url);

            
            
            return new OkObjectResult(article);
        }
    }
}
