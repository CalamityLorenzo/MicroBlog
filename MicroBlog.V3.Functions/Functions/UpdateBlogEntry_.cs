using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Functions.Models.App;
using MicroBlog.V3.Functions.Settings;
using MicroBlog.V3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MicroBlog.V3.Functions
{
    public static class UpdateBlogEntry_
    {
        [FunctionName("UpdateBlogEntry")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("UpdatedBlogEntry function processed a request.");

            var article = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            var articleInput = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            var mOpts = AppConfigSettings.GetOptions();

            var bas = new BlogArticleService(new CloudStorageContext(mOpts.StorageAccount), mOpts, log);

            var updatedArticle = await bas.Update(article);
            return new OkResult();
        }
    }
}
