using MicroBlog.V3.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Services.Models
{
    class ArticleFileData : IArticle
    {
        public ArticleFileData(IClientArticle article) : this(article.Title, article.Article,  article.Id){}

        public ArticleFileData(IClientArticle article, Guid Id) : this(article.Title, article.Article, Id) { }

        [JsonConstructor]
        public ArticleFileData(string title, string articleText,  Guid id)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            ArticleText = articleText ?? throw new ArgumentNullException(nameof(articleText));
            Id = id;
        }

        public string Title { get; }

        public string ArticleText { get; }

        public Guid Id { get; }
    }
}
