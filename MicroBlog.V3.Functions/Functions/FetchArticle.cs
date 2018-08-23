
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

namespace MicroBlog.V3.Functions
{
    public static class FetchArticle
    {
        [FunctionName("FetchArticle")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("CreateBlogEntry processed a request.");
            
            var articleInput = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);

            // You can either retrieve an article by Id or it's relative Url
            var blogService = ArticleService.GetManager();
            var tagService = TagService.GetManager();
            var categoryService = CategoryService.GetManager();

            BaseArticle article = (articleInput.Url.Length == 0) ? new BaseArticle(await blogService.Get(articleInput.Id)) : new BaseArticle(await blogService.GetByUrl(articleInput.Url));

            var Tasks = new Task[] {
                tagService.Get(article.Id),
                categoryService.Get(article.Id)
            };
            Task.WaitAll(Tasks);

            var tagTask = Tasks[0] as Task<IArticleTags>;
            var tagResult = tagTask.Result;

            var catTask = Tasks[1] as Task<IArticleCategories>;
            var catResults = catTask.Result;
            return new OkObjectResult(new CompleteClientArticle(article, 
                                      new ArticleTags(tagResult.Tags, article.Id), 
                                      new ArticleCategories(catResults.Tags, article.Id)));
        }
    }
}
