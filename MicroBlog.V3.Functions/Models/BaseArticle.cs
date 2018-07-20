using MicroBlog.V3.Interfaces;
using System;

namespace MicroBlog.V3.Functions.Models
{
    internal class BaseArticle : IClientArticle

    {
        public BaseArticle(ICompleteArticle totalEntry)
        {
            this.Id = totalEntry.Id;
            this.Url = totalEntry.Url;
            this.Synopsis = totalEntry.Synopsis;
            this.Published = totalEntry.Published;
            this.Created = totalEntry.Created;
            this.Author = totalEntry.Author;
            this.Article = totalEntry.Article;
            this.Title = totalEntry.Title;     
        }

        public Guid Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Article { get; set; }

        public string Synopsis { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Published { get; set; }
    }
}
