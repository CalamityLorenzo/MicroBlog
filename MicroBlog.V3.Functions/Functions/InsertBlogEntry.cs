
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

namespace MicroBlog.V3.Functions
{
    public static class InsertBlogEntry
    {
        [FunctionName("InsertBlogEntry")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("CreateBlogEntry processed a request.");

            var completeArticle = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            // Deconstruct
            (var article, var tags, var categories) = completeArticle;

            var blogService = ArticleService.GetManager();
            var tagService = TagService.GetManager();
            var categoryService = CategoryService.GetManager();
            var newEntry = await blogService.Create(article);
            var savedTags = await tagService.Create(tags.Tags, newEntry.Id);
            var savedCategories = await categoryService.Create(categories.Tags, newEntry.Id);

            return new OkObjectResult(new CompleteClientArticle(newEntry, savedTags, savedCategories));
        }
    }
}
