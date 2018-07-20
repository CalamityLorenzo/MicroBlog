using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestBlogv3.Models
{
    class BlogEntry : IClientArticle
    {
        public BlogEntry(string url, string title, string synopsis, string article, string author, DateTime created, DateTime? published)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Synopsis = synopsis ?? throw new ArgumentNullException(nameof(synopsis));
            Article = article ?? throw new ArgumentNullException(nameof(article));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Created = created;
            Published = published;
        }

        public BlogEntry(IClientArticle article) : this(article.Url, article.Title, article.Synopsis, article.Article,  article.Author, article.Created, article.Published) {
            this.Id = article.Id;
        }

        public string Url {get;}
        public string Title {get;}
        public string Synopsis {get;}
        public string Article {get;}
        public string Author {get;}
        public DateTime Created {get;}
        public DateTime? Published {get;}
        public Guid Id {get;}

        public override string ToString()
        {
            return $"{Id} {Title}";
        }
    }
}
