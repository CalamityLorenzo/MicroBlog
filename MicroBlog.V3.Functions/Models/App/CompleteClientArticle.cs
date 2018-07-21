using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Functions.Models.App
{
    class CompleteClientArticle : ICompleteArticle
    {
        public CompleteClientArticle(){}

        public CompleteClientArticle(string url, string title, string synopsis, string article, string author, IEnumerable<string>Tags, IEnumerable<string>Categories,  DateTime created, DateTime? published)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Synopsis = synopsis ?? throw new ArgumentNullException(nameof(synopsis));
            Article = article ?? throw new ArgumentNullException(nameof(article));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            this.Tags = new List<string>(Tags);
            this.Categories = new List<string>(Categories);
            Created = created;
            Published = published;
        }

        public CompleteClientArticle(IClientArticle article, IArticleTags tags, IArticleCategories categories) : this(article.Url, article.Title, article.Synopsis, article.Article, article.Author, tags.Tags, categories.Tags, article.Created, article.Published)
        {
            Id = article.Id;
        }
        public void Deconstruct(out IClientArticle clientArticle, out IArticleTags tags, out IArticleCategories cats)
        {
            clientArticle = new BaseArticle(this);
            tags = new ArticleTags(this);
            cats = new ArticleCategories(this);
        }

        public Guid Id {get;set;}

        public string Url {get;set;}

        public string Title {get;set;}

        public string Article {get;set;}

        public string Synopsis {get;set;}

        public string Author {get;set;}

        public IEnumerable<string> Tags {get;set;}

        public IEnumerable<string> Categories {get;set;}

        public DateTime Created {get;set;}

        public DateTime? Published {get;set;}
    }
}
