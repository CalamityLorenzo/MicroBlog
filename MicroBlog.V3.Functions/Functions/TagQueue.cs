using System;
using MicroBlog.V3.Entities.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MicroBlog.V3.Functions.Functions
{
    public static class TagQueue
    {
        [FunctionName("TagQueue")]
        public static void Run([QueueTrigger("%TagQueue%", Connection = "%AzureWebJobsStorage%")]QueueMessage myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
