using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Services;
using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBlogv3.Models;

namespace TestBlogv3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IConfigurationBuilder build = new ConfigurationBuilder()
                         .SetBasePath(Environment.CurrentDirectory)
                         .AddJsonFile("config.json", false, true);
            IConfigurationRoot config = build.Build();
            IConfigurationSection appKeys = config.GetSection("appSettings");
            ILoggerFactory loggerFactory = new LoggerFactory()
                                        .AddConsole(LogLevel.Trace)
                                        .AddDebug();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            DoThings(appKeys, logger).Wait();
        }

        private static async Task DoThings(IConfigurationSection appKeys, ILogger logger)
        {
            MicroBlogConfiguration.SetConfiguration(() => new MicroBlogConfiguration.MicroBlogOptions(appKeys["StorageAcc"], appKeys["NewArticleStore"], appKeys["NewArticleDetails"], appKeys["TagProcessing"], appKeys["TagStore"], appKeys["catQueue"], appKeys["catStore"]));
            var blogOh1 = new CompleteBlogEntry("https", "string bling", "synthOio", "article", "Pal lawrence", new List<string> { "Salad", "Fortune", "Cava" }, new List<string> { "Science", "MAths", "Boojagger" }, DateTime.Now, DateTime.Today.AddDays(-100));

            // In the function, we deconstruct the original message
            var allOpts = MicroBlogConfiguration.GetOptions();
            var cloudAcct = new CloudStorageContext(allOpts.StorageAccount);



            // In a whorld where this is a functionapp.
            // we pass this entire BlogPost into  function
            BlogArticleService bas = new BlogArticleService(cloudAcct, allOpts, logger);
        }
    }
}
