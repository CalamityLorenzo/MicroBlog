using MicroBlog.V3.Services;
using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
            MicroBlogConfiguration.SetConfiguration(() => new MicroBlogConfiguration.Options(appKeys["StorageAcc"], appKeys["NewArticleStore"], appKeys["NewArticleDetails"], appKeys["TagProcessing"], appKeys["TagStore"]));
            var bs = BlogService.GetManager();
            var ts = TagService.GetManager();
            var blogOh1 = new BlogEntry("https", "string bling", "synthOio", "article", "Pal lawrence", DateTime.Now, DateTime.Today.AddDays(-100));
            var entity = await bs.Create(blogOh1);
            var tags = ts.Create(new List<string> { "Salad", "Fortune", "Cava" }, entity.Id);
            var results = new BlogEntry(await bs.Get(entity.Id));
            Console.WriteLine(results);
        }
    }
}
