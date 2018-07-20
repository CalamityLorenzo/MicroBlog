using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace MicroBlog.V3.Functions
{
    public static class Function1
    {
        [FunctionName("tagprocess")]
        public static void Run([QueueTrigger("TagProcessing", Connection = "")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
