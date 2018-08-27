using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Entities.Models
{
    public class CompleteBlogEntry : ICompletePost
    {
        public CompleteBlogEntry(string url, string title, string synopsis, string article, string author, IEnumerable<string> tags, IEnumerable<string> categories, DateTime created, DateTime? published)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Synopsis = synopsis ?? throw new ArgumentNullException(nameof(synopsis));
            Article = article ?? throw new ArgumentNullException(nameof(article));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            this.Tags = new List<string>(tags);
            this.Categories = new List<string>(categories);
            Created = created;
            Published = published;
        }

        internal CompleteBlogEntry(Guid id, string url, string title, string synopsis, string article, string author, IEnumerable<string> tags, IEnumerable<string> categories, DateTime created, DateTime? published) : this(url, title, synopsis, article, author, tags, categories, created, published)
        {
            this.Id = id;
        }

        public CompleteBlogEntry(IClientArticle article, IArticleTags tags, IArticleCategories categories) : this(article.Url, article.Title, article.Synopsis, article.Article, article.Author, tags.Tags, categories.Tags, article.Created, article.Published)
        {
            Id = article.Id;
        }

        public CompleteBlogEntry(ICompletePost article) : this(article.Url, article.Title, article.Synopsis, article.Article, article.Author, article.Tags, article.Categories, article.Created, article.Published)
        {
            Id = article.Id;
        }

        public void Deconstruct(out IClientArticle clientArticle, out IArticleTags tags, out IArticleCategories cats)
        {
            clientArticle = new ClientArticle(this);
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

    [CreateMatchMethods(typeof(CompleteBlogEntry))]
    public static class CompleteBlogEntryExtensionMethods
    {
        public static CompleteBlogEntry WithTitle(this CompleteBlogEntry article, string Title)
        {
            return new CompleteBlogEntry(article.Id, article.Url, Title, article.Synopsis, article.Article, article.Author, article.Tags, article.Categories, article.Created, article.Published);
        }
        public static CompleteBlogEntry WithSynopsis(this CompleteBlogEntry article, string Synopsis)
        {
            return new CompleteBlogEntry(article.Id, article.Url, article.Title, Synopsis, article.Article, article.Author, article.Tags, article.Categories, article.Created, article.Published);
        }
        public static CompleteBlogEntry WithArticle(this CompleteBlogEntry article, string Article)
        {
            return new CompleteBlogEntry(article.Id, article.Url, article.Title, article.Synopsis, Article, article.Author, article.Tags, article.Categories, article.Created, article.Published);
        }
        public static CompleteBlogEntry WithAuthor(this CompleteBlogEntry article, string Author)
        {
            return new CompleteBlogEntry(article.Id, article.Url, article.Title, article.Synopsis, article.Article, article.Author, article.Tags, article.Categories, article.Created, article.Published);
        }
        public static CompleteBlogEntry WithTags(this CompleteBlogEntry article, List<string> Tags)
        {
            return new CompleteBlogEntry(article.Id, article.Url, article.Title, article.Synopsis, article.Article, article.Author, Tags, article.Categories, article.Created, article.Published);
        }
        public static CompleteBlogEntry WithCategories(this CompleteBlogEntry article, List<string> Categories)
        {
            return new CompleteBlogEntry(article.Id, article.Url, article.Title, article.Synopsis, article.Article, article.Author, article.Tags, Categories, article.Created, article.Published);
        }

    }
}
