using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Entities.Models;
using MicroBlog.V3.Interfaces;
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
                         .AddJsonFile("local.settings.json ", false, true);
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
            var allOpts = MicroBlogConfiguration.GetOptions();
            var cloudAcct = new CloudStorageContext(allOpts.StorageAccount);
        
            var blogOh1 = new CompleteBlogEntry("my-new-url", "The first Title I Choosed",
                                                "On top of the old help finishes an adult handicap. When can the drivel chew? How does the senior priest do the skip? Why can't a backlog pile a concentrate? The saga wins the proprietary equilibrium. The arrogance sponsors the jazz.", "article", 
                                                "Paul lawrence", 
                                                new List<string> { "KEllogs", "Tonka", "Roos" }, 
                                                new List<string> { "New-Science", "Killer-Bees", "Rune Doogle" }, 
                                                DateTime.Now, DateTime.Today.AddDays(-100));

            // In the function, we deconstruct the original message

            // In a whorld where this is a functionapp.
            // we pass this entire BlogPost into  function
            BlogArticleService bas = new BlogArticleService(cloudAcct, allOpts, logger);
            var newPost = await CreatePost(bas, blogOh1);

            var editedPost = new  CompleteBlogEntry(newPost).WithTitle("Jelly bean fiend");

            var ratherEditedPost = await UpdatePost(bas, editedPost);
            logger.LogInformation($"{ratherEditedPost.Id} {ratherEditedPost.Url}");
            var furtherEditedPost = await UpdatePost(bas, ratherEditedPost);
            logger.LogInformation($"{furtherEditedPost.Id} {furtherEditedPost.Url}");


        }

        private static async Task<ICompletePost> UpdatePost(BlogArticleService bas, ICompletePost blogPost)
        {

            return await bas.Update(blogPost);
        }

        private static async Task<ICompletePost> CreatePost(BlogArticleService bas, CompleteBlogEntry blogOh1)
        {

           return await bas.Add(blogOh1);
        }
    }
}
