using MicroBlog.V3.Entities.Interfaces;
using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
namespace MicroBlog.V3.Entities.Models
{
    public class ClientArticle : IClientArticle
    {
        // Creates an empty shell of article
        // explistlyees
        internal ClientArticle()
        {
            Id = Guid.Empty;
            (Url, Title, Synopsis, Article) = ("", "", "", "");
            Published = null;
            Created = DateTime.MinValue;

        }

        public ClientArticle(ICompletePost article, Guid id)
        {
            Url = article.Url;
            Title = article.Title;
            Synopsis = article.Synopsis;
            Author = article.Author;
            Created = article.Created;
            Published = article.Published;
            Article = article.Article;
            Updated = article.Updated;
            Available = article.Available;
            Id = id;

        }
        public ClientArticle(ICompletePost post)
        {
            Url = post.Url;
            Title = post.Title;
            Synopsis = post.Synopsis;
            Author = post.Author;
            Created = post.Created;
            Published = post.Published;
            Article = post.Article;
            Updated = post.Updated;
            Available = post.Available;
            Id = post.Id;

        }
        public ClientArticle(IClientArticle article)
        {
            Url = article.Url;
            Title = article.Title;
            Synopsis = article.Synopsis;
            Author = article.Author;
            Created = article.Created;
            Published = article.Published;
            Article = article.Article;
            Updated = article.Updated;
            Available = article.Available;
            Id = article.Id;

        }

        public ClientArticle(IArticle article, IArticleDetails details)
        {
            Url = details.Url;
            Title = article.Title;
            Synopsis = details.Synopsis;
            Author = details.Author;
            Created = details.Created;
            Published = details.Published;
            Updated = details.Updated;
            Available = details.Available;
            Article = article.ArticleText;
            Id = article.Id;
        }

        public ClientArticle(IClientArticle article, DateTime? published) : this(article)
        {
            Published = published;
        }

        public string Url { get; private set; }
        public string Title { get; private set; }
        public string Synopsis { get; private set; }
        public string Article { get; private set; }
        public string Author { get; private set; }
        public DateTime Updated { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Published { get; private set; }
        public bool Available { get; private set; }
        public Guid Id { get; private set; }
    }

    
    public static class ClientArticleExtensionMethods
    {
        public static IClientArticle WithPublished(this IClientArticle @this, DateTime? published)
        {
            return new ClientArticle(@this, published);
        }
    }
}
