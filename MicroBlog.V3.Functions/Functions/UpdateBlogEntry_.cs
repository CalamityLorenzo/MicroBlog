using MicroBlog.V3.Functions.Models.App;
using MicroBlog.V3.Functions.Settings;
using MicroBlog.V3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace MicroBlog.V3.Functions
{
    public static class UpdateBlogEntry_
    {
        [FunctionName("UpdateBlogEntry")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("UpdatedBlogEntry function processed a request.");

            CompleteClientArticle updatedArticle = AppConfigSettings.IngestRequest<CompleteClientArticle>(req, context);
            // Deconstruct
            (Interfaces.IClientArticle article, Interfaces.IArticleTags tags, Interfaces.IArticleCategories categories) = updatedArticle;

            Interfaces.IArticleService bs = ArticleService.GetManager();
            Interfaces.ITagService ts = TagService.GetManager();
            Interfaces.ICategoryService cs = CategoryService.GetManager();
            Task[] tasks = new Task[] {
            bs.Update(article),
            ts.Update(tags),
            cs.Update(categories)
            };

            Task.WaitAll();


            return new OkResult();
        }
    }
}
