using MicroBlog.V3.Interfaces;
using System;

namespace MicroBlog.V3.Functions.Models.App
{
    internal class BaseArticle : IClientArticle

    {
        public BaseArticle(ICompletePost totalArticle)
        {
            this.Id = totalArticle.Id;
            this.Url = totalArticle.Url;
            this.Synopsis = totalArticle.Synopsis;
            this.Published = totalArticle.Published;
            this.Created = totalArticle.Created;
            this.Author = totalArticle.Author;
            this.Article = totalArticle.Article;
            this.Title = totalArticle.Title;
            this.Updated = totalArticle.Updated;
            this.Available = totalArticle.Available;
        }

        public BaseArticle(IClientArticle article)
        {
            this.Id = article.Id;
            this.Url = article.Url;
            this.Synopsis = article.Synopsis;
            this.Published = article.Published;
            this.Created = article.Created;
            this.Author = article.Author;
            this.Article = article.Article;
            this.Title = article.Title;
            this.Updated = article.Updated;
            this.Available = article.Available;
        }

        public Guid Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Article { get; set; }

        public string Synopsis { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Published { get; set; }
        public bool Available { get; set; }
    }
}
