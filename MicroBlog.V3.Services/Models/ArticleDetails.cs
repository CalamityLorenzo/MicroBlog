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
        public ArticleDetails(IClientArticle article) : this(article.Url, article.Title, article.Synopsis, article.Author, article.Created, article.Published, article.Available, article.Updated, article.Id) { }
        public ArticleDetails(IClientArticle article, Guid Id) : this(article.Url, article.Title, article.Synopsis, article.Author, article.Created, article.Published, article.Available, article.Updated, Id) { }
        public ArticleDetails(string url, string title, string synopsis, string author, DateTime created, DateTime? published, bool available, DateTime updated, Guid id)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Synopsis = synopsis ?? throw new ArgumentNullException(nameof(synopsis));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Created = created;
            Published = published;
            this.PartitionKey = id.ToString();
            this.RowKey = url;
            this.Available = available;
            this.Updated = updated;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
        }

        public string Url
        {
            get => this.RowKey;
            set { this.RowKey = value; }
        }

        public string Title { get; set; }

        public string Synopsis { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Published { get; set; }

        public bool Available { get; set; }

        public DateTime Updated { get; set; }

        public Guid Id => Guid.Parse(PartitionKey);
        public string Type { get => "IdUrl"; }
    }

    // We store the Url as seperate row/parition key along with the guid id.
    // So we can quickly lookup by Url. It requires a second query to get the article. 
    // But I can live with tha.
    class ArticleDetailsUrlId : TableEntity
    {
        public ArticleDetailsUrlId()
        {
        }


        public ArticleDetailsUrlId(string url, Guid Id) : base(url, Id.ToString())
        {

        }

        public string Url { get => PartitionKey; }
        public string Id { get => RowKey; }
    }
}
