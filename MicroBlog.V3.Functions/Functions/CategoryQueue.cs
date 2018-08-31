using System;
using MicroBlog.V3.Entities.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MicroBlog.V3.Functions.Functions
{
    public static class CategoryQueue
    {
        [FunctionName("CategoryQueue")]
        public static void Run([QueueTrigger("%CatQueue%", Connection = "%AzureWebJobsStorage%")]QueueMessage myQueueItem, ILogger log)
        {
            log.LogInformation($"Category Queue trigger function processed: {myQueueItem.Status}");
        }
    }
}
