using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;

namespace TestBlogv3.Models
{
    internal class ArticleCategories : IArticleCategories
    {

        public ArticleCategories() { }

        public ArticleCategories(IEnumerable<string> categories, Guid Id)
        {
            this.Id = Id;
            Tags = categories;
        }
        public ArticleCategories(ICompletePost article)
        {
            Id = article.Id;
            Tags = new List<string>(article.Tags);
        }
        public Guid Id { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
