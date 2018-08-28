using MicroBlog.V3.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using TestBlogv3.TextData;
namespace TestBlogv3
{
    class BlogPostTests
    {
        public IEnumerable<CompleteBlogEntry> CreateFakePosts()
        {
            var results = new List<CompleteBlogEntry>();

            var createdUpdated = DateTime.Now;
            for (var x = 0; x < 10; ++x)
            {
                results.Add(
                    new CompleteBlogEntry(
                        roodData.UrlTitle[x].url,
                        roodData.UrlTitle[x].title,
                        roodData.GetSynopsis(x + 1),
                        roodData.GetBodyText(x + 1),
                        "Paul Lawrence",
                        roodData.Tags[x],
                        roodData.Categories[x],
                        createdUpdated,
                        roodData.Published[x],
                        createdUpdated,
                        roodData.Available[x]
                    ));
            }

            return results;
        }
    }
}
