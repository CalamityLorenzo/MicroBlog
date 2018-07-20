using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;

namespace TestBlogv3.Models
{
    internal class BlogEntry : ICompleteArticle
    {
        public BlogEntry(string url, string title, string synopsis, string article, string author, IEnumerable<string>Tags, IEnumerable<string>Categories,  DateTime created, DateTime? published)
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

        public BlogEntry(IClientArticle article, IArticleTags tags, IArticleCategories categories) : this(article.Url, article.Title, article.Synopsis, article.Article, article.Author, tags.Tags, categories.Tags, article.Created, article.Published)
        {
            Id = article.Id;
        }

        public BlogEntry(ICompleteArticle article) : this(article.Url, article.Title, article.Synopsis, article.Article, article.Author, article.Tags, article.Categories, article.Created, article.Published)
        {
            Id = article.Id;
        }

        public void Deconstruct(out IClientArticle clientArticle, out IArticleTags tags, out IArticleCategories cats)
        {
            clientArticle = new BasicArticle(this);
            tags = new ArticleTags(this);
            cats = new ArticleCategories(this);
        }

        public string Url { get; }
        public string Title { get; }
        public string Synopsis { get; }
        public string Article { get; }
        public string Author { get; }
        public DateTime Created { get; }
        public DateTime? Published { get; }
        public Guid Id { get; }

        public IEnumerable<string> Tags { get; }

        public IEnumerable<string> Categories { get; }

        public override string ToString()
        {
            return $"{Id} {Title}";
        }
    }
}
