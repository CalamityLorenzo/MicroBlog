using MicroBlog.V3.Interfaces;
using System;

namespace MicroBlog.V3.Services.Models
{
    internal class CompleteArticle : IClientArticle
    {

        public CompleteArticle(IClientArticle article, Guid id)
        {
            Url = article.Url;
            Title = article.Title;
            Synopsis = article.Synopsis;
            Author = article.Author;
            Created = article.Created;
            Published = article.Published;
            Article = article.Article;
            Id = id;

        }

        public CompleteArticle(IArticle article, IArticleDetails details)
        {
            Url = details.Url;
            Title = article.Title;
            Synopsis = details.Synopsis;
            Author = details.Author;
            Created = details.Created;
            Published = details.Published;
            Article = article.ArticleText;
            Id = article.Id;
        }

        public string Url { get; private set; }
        public string Title { get; private set; }
        public string Synopsis { get; private set; }
        public string Article { get; private set; }
        public string Author { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Published { get; private set; }
        public Guid Id { get; private set; }
    }
}
