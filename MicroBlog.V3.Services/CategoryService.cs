using AzureStorage.V2.Helpers;
using AzureStorage.V2.Helpers.Context;
using AzureStorage.V2.Helpers.SimpleStorage;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using MicroBlog.V3.Services.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static AzureStorage.V2.Helpers.Context.CloudStorageContext;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration.MicroBlogOptions;

namespace MicroBlog.V3.Services
{
    public class CategoryService : ICategoryService
    {
        private LazyAsync<SimpleTableHelper> tagTableStore;
        private LazyAsync<SimpleQueueHelper> tagQueueStore;
        private readonly CloudStorageContext cscCtx;
        private readonly ILogger logger;

        internal  CategoryService(CloudStorageContext cscCtx, MicroBlogOptions opts, ILogger logger)
        {
            tagTableStore = new LazyAsync<SimpleTableHelper>( async ()=>await cscCtx.CreateTableHelper(opts[StorageList.CategoryTable]));
            tagQueueStore = new LazyAsync<SimpleQueueHelper>(async () => await cscCtx.CreateQueueHelper(opts[StorageList.CategoryQueue]));
            this.cscCtx = cscCtx;
            this.logger = logger;
        }

        public Task<IArticleCategories> Create(IEnumerable<string> tags, Guid Id)
        {
            return this.Create(new ArticleCategories(tags, Id));
        }

        public async Task<IArticleCategories> Create(IArticleCategories Entity)
        {
            var tagTable = await tagTableStore.Value;
            var tagQueue = await tagQueueStore.Value;
            await tagTable.Insert(new ArticleCategories(Entity));
            // no wait! We also need to update statistics
            await tagQueue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = Entity.Id, Status = QueueMessageStatus.Added }));
            return Entity;
        }

        public async Task Delete(IArticleCategories Entity)
        {
            await this.Delete(Entity.Id);
        }

        public async Task Delete(Guid EntityId)
        {
            var tagTable = await tagTableStore.Value;
            var tagQueue = await tagQueueStore.Value;
            var stringId = EntityId.ToString();
            var tag = await tagTable.Get<ArticleTags>(stringId, stringId);
            await tagTable.Delete(tag);
            // update statistics.
            await tagQueue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = EntityId, Status = QueueMessageStatus.Deleted }));
        }

        public async Task<IArticleCategories> Get(Guid EntityId)
        {
            var tagTable = await tagTableStore.Value;
            
            var stringId = EntityId.ToString();
            var cats = await tagTable.Get<ArticleCategories>(stringId, stringId);
            return cats ?? new ArticleCategories(EntityId);
        }

        public async Task<IArticleCategories> Update(IArticleCategories Entity)
        {
            var tagTable = await tagTableStore.Value;
            var updated = new ArticleCategories(Entity);
            await tagTable.Replace(updated);

            return updated;
        }
        
    }
}
