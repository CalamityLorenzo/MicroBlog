using AzureStorage.V2.Helpers;
using AzureStorage.V2.Helpers.Context;
using AzureStorage.V2.Helpers.SimpleStorage;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using MicroBlog.V3.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AzureStorage.V2.Helpers.Context.CloudStorageContext;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration.MicroBlogOptions;

namespace MicroBlog.V3.Services
{
    public class TagService : ITagService
    {
        private LazyAsync<SimpleTableHelper> tagTableStore;
        private LazyAsync<SimpleQueueHelper> tagQueueStore;
        private readonly CloudStorageContext cscCtx;

        internal TagService(CloudStorageContext cscCtx, MicroBlogOptions opts)
        {
            tagTableStore = new LazyAsync<SimpleTableHelper>(async () => await cscCtx.CreateTableHelper(opts[StorageList.TagTable]));
            tagQueueStore = new LazyAsync<SimpleQueueHelper>(async () => await cscCtx.CreateQueueHelper(opts[StorageList.TagQueue]));
            this.cscCtx = cscCtx;
        }

        public Task<IArticleTags> Create(IEnumerable<string> tags, Guid Id)
        {
            return Create(new ArticleTags(tags, Id));
        }

        public async Task<IArticleTags> Create(IArticleTags Entity)
        {
            var tagTable = await tagTableStore.Value;
            var tagQueue = await tagQueueStore.Value;
            await tagTable.Insert(new ArticleTags(Entity));
            // no wait! We also need to update statistics
            await tagQueue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = Entity.Id, Status = QueueMessageStatus.Added }));
            return Entity;
        }

        public async Task Delete(IArticleTags Entity)
        {
            await Delete(Entity.Id);
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

        public async Task<IArticleTags> Get(Guid EntityId)
        {
            var tagTable = await tagTableStore.Value;

            var stringId = EntityId.ToString();
            var tags = await tagTable.Get<ArticleTags>(stringId, stringId);
            return tags ?? new ArticleTags(EntityId);
        }

        public async Task<IArticleTags> Update(IArticleTags Entity)
        {
            var tagTable = await tagTableStore.Value;
            var tagQueue = await tagQueueStore.Value;
            var inserted = new ArticleTags(Entity);
            await tagTable.Replace(inserted);
            await tagQueue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = Entity.Id, Status = QueueMessageStatus.Updated }));

            return inserted;
        }

        public static ITagService GetManager()
        {
            var opts = MicroBlogConfiguration.GetOptions();
            return new TagService(new CloudStorageContext(opts.StorageAccount), opts);
        }
    }

}
