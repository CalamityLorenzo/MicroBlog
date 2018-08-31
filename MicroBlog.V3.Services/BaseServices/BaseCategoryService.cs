using AzureStorage.V2.Helpers;
using AzureStorage.V2.Helpers.Context;
using AzureStorage.V2.Helpers.SimpleStorage;
using MicroBlog.V3.Entities.Models;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AzureStorage.V2.Helpers.Context.CloudStorageContext;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;

namespace MicroBlog.V3.Services.BaseServices
{
    /// <summary>
    /// A way of storing a chunk of words against a blog post.
    /// These can be further processed. with a Queue trigger too.
    /// </summary>
    public abstract class BaseCategoryService : ICategoryService
    {
        protected LazyAsync<SimpleTableHelper> _tableStore;
        protected LazyAsync<SimpleQueueHelper> _QueueStore;
        protected readonly CloudStorageContext cscCtx;
        protected readonly ILogger logger;

        internal BaseCategoryService(CloudStorageContext cscCtx, MicroBlogOptions opts, ILogger logger, string tableName, string queueName)
        {
            this.logger = logger;
            this._tableStore = new LazyAsync<SimpleTableHelper>(async () => await cscCtx.CreateTableHelper(tableName, logger));
            _QueueStore = new LazyAsync<SimpleQueueHelper>(async () => await cscCtx.CreateQueueHelper(queueName, logger));
            this.cscCtx = cscCtx;
        }

        public Task<IArticleCategories> Create(IEnumerable<string> tags, Guid Id)
        {
            return this.Create(new ArticleCategoryTableEntity(tags, Id));
        }

        public async Task<IArticleCategories> Create(IArticleCategories Entity)
        {
            var tagTable = await _tableStore.Value;
            var queue = await _QueueStore.Value;
            await tagTable.Insert(new ArticleCategoryTableEntity(Entity));
            // no wait! We also need to update statistics
            await queue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = Entity.Id, Status = QueueMessageStatus.Added }));

            return Entity;
        }

        public async Task Delete(IArticleCategories Entity)
        {
            await this.Delete(Entity.Id);
        }

        public async Task Delete(Guid EntityId)
        {
            var tagTable = await _tableStore.Value;
            var queue = await _QueueStore.Value;
            var stringId = EntityId.ToString();
            var tag = await tagTable.Get<ArticleTagsTableEntity>(stringId, stringId);

            await tagTable.Delete(tag);
            // update statistics.
            await queue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = EntityId, Status = QueueMessageStatus.Deleted }));

        }

        public async Task<IArticleCategories> Get(Guid EntityId)
        {
            var tagTable = await _tableStore.Value;

            var stringId = EntityId.ToString();
            var cats = await tagTable.QueryByPartitionKey<ArticleCategoryTableEntity>(EntityId.ToString(), 1);
            // return null if no categories found
            return cats != null ? new ArticleCategoryTableEntity(cats.First().Tags.ToList(), EntityId) : new ArticleCategoryTableEntity(EntityId);
        }

        public async Task<IArticleCategories> Update(IArticleCategories Entity)
        {
            // updating here means, checking to see if we actually had any to begin with.
            // If not then we are actually adding!.
            var existing = await this.Get(Entity.Id);
            var tagTable = await _tableStore.Value;
            var queue = await _QueueStore.Value;
            if (existing != null)
            {
                var updated = new ArticleCategoryTableEntity(Entity) { ETag = "*" };
                await tagTable.Replace(updated);
            }
            else
            {
                await this.Create(Entity);
            }

            await queue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = Entity.Id, Status = QueueMessageStatus.Updated }));
            return Entity;


        }

    }
}
