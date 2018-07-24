using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using MicroBlog.V3.Functions.Models;
using MicroBlog.V3.Functions.Settings;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services;


namespace MicroBlog.V3.Functions.Functions
{
    public static class GetArticleHeaders
    {
        [FunctionName("GetArticleHeaders")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("GetArticleHeaders function processed a request.");

            var headersQuery = AppConfigSettings.IngestRequest<GetArticleHeadersInput>(req, context);

            List<IArticleDetails> results = new List<IArticleDetails>();

            var bs = BlogService.GetManager();
            results.AddRange(await bs.FindArticlDetails(headersQuery.Start, headersQuery.End, headersQuery.Take, headersQuery.Skip));

            return new OkObjectResult(results);
        }
    }
}
