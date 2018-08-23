using MicroBlog.V3.Services;
using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Configuration;
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
            DoThings(appKeys).Wait();
        }

        private static async Task DoThings(IConfigurationSection appKeys)
        {
            MicroBlogConfiguration.SetConfiguration(() => new MicroBlogConfiguration.Options(appKeys["StorageAcc"], appKeys["NewArticleStore"], appKeys["NewArticleDetails"], appKeys["TagProcessing"], appKeys["TagStore"], appKeys["catQueue"], appKeys["catStore"]));

            // In a whorld where this is a functionapp.
            // we pass this entire BlogPost into  function
            var blogOh1 = new CompleteBlogEntry("https", "string bling", "synthOio", "article", "Pal lawrence", new List<string> { "Salad", "Fortune", "Cava" }, new List<string> { "Science", "MAths", "Boojagger" }, DateTime.Now, DateTime.Today.AddDays(-100));

            // In the function, we deconstruct the original message
            (var article, var tags, var categories) = blogOh1;
            var bs = ArticleService.GetManager();
            var ts = TagService.GetManager();
            // INsert and create a new Blog Article
            var entity = await bs.Create(article);
            // Ditto the tsg, and categories

            var createdTags = await ts.Create(tags.Tags.ToList(), entity.Id);
            var results = new CompleteBlogEntry(await bs.Get(entity.Id), createdTags,
                                    new ArticleCategories(categories.Tags, entity.Id));
            Console.WriteLine(results);
        }
    }
}
