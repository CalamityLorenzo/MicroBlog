﻿using MicroBlog.V3.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroBlog.V3.Services.Models
{
    class ArticleCategories : TableEntity, IArticleCategories
    {
        public ArticleCategories()
        {
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            // Serialising List entities into fields is a tricksy business
            // so We make it a custom thang.
            var leDic = base.WriteEntity(operationContext);
            leDic.Add(nameof(Tags), EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(this.Tags)));
            return leDic;
        }

        public ArticleCategories(IArticleCategories tags) : this(tags.Tags, tags.Id) { }

        public ArticleCategories(IEnumerable<string> Tags, Guid Id)
        {
            this.Tags = new HashSet<string>(Tags);
            this.PartitionKey = Id.ToString();
            this.RowKey = Id.ToString();
        }


        public Guid Id { get => Guid.Parse(RowKey); }
        public IEnumerable<string> Tags { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            // Again we have to custom manage the list/Ienumerable types 
            base.ReadEntity(properties, operationContext);
            this.Tags = JsonConvert.DeserializeObject<List<string>>(properties["Tags"].StringValue);
        }

        private static Lazy<ArticleCategories> EmptyVersion = new Lazy<ArticleCategories>(() => new ArticleCategories(new List<string>(), Guid.Empty));

        internal static ArticleCategories Empty()
        {
            return EmptyVersion.Value;
        }
    }
}
