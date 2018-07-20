using MicroBlog.V3.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroBlog.V3.Services.Models
{
    class ArticleDetails : TableEntity, IArticleDetails, ITableColumns
    {
        public IEnumerable<string> SelectColumns => new List<string>(this.GetType().GetProperties().Select(o => o.Name));

        public ArticleDetails() { }
        public ArticleDetails(IClientArticle article) : this(article.Url, article.Title, article.Synopsis, article.Author, article.Created, article.Published, article.Id) { }
        public ArticleDetails(IClientArticle article, Guid Id) : this(article.Url, article.Title, article.Synopsis, article.Author, article.Created, article.Published, Id) { }
        public ArticleDetails(string url, string title, string synopsis, string author, DateTime created, DateTime? published, Guid id)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Synopsis = synopsis ?? throw new ArgumentNullException(nameof(synopsis));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Created = created;
            Published = published;
            this.PartitionKey = id.ToString();
            this.RowKey = ArticleDetails.RowKeyDef;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
        }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Synopsis { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Published { get; set; }

        public Guid Id => Guid.Parse(PartitionKey);
        public static string RowKeyDef => "BlogArticle";

    }

}
