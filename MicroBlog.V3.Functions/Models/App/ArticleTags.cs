using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Functions.Models.App
{
    class ArticleTags : IArticleCategories
    {

        public ArticleTags(IEnumerable<string> tags, Guid Id)
        {
            this.Id = Id;
            Tags = tags;
        }


        public ArticleTags() { }
        public ArticleTags(ICompletePost article)
        {
            Id = article.Id;
            Tags = article.Tags;
        }


        public Guid Id {get;set;}

        public IEnumerable<string> Tags {get;set;}
    }
}
