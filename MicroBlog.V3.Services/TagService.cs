using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using MicroBlog.V3.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AzureStorage.V2.Helpers.Context.CloudStorageContext;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;

namespace MicroBlog.V3.Services
{
    public class TagService : ITagService
    {
        private SimpleTableHelper tagTable;
        private SimpleQueueHelper tagQueue;
        private CloudStorageContext cscCtx;

        internal TagService(CloudStorageContext cscCtx, Options opts)
        {
            tagTable = cscCtx.CreateTableHelper(opts.TagTable);
            tagQueue = cscCtx.CreateQueueHelper(opts.TagQueue);
            this.cscCtx = cscCtx;
        }

        public Task<IArticleTags> Create(IEnumerable<string> tags, Guid Id)
        {
            return Create(new ArticleTags(tags, Id));
        }

        public async Task<IArticleTags> Create(IArticleTags Entity)
        {
            await tagTable.InsertToTable(new ArticleTags(Entity));
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
            var stringId = EntityId.ToString();
            var tag = await tagTable.GetEntity<ArticleTags>(stringId, stringId);
            await tagTable.DeleteEntity(tag);
            // update statistics.
            await tagQueue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = EntityId, Status = QueueMessageStatus.Deleted }));
        }

        public async Task<IArticleTags> Get(Guid EntityId)
        {
            var stringId = EntityId.ToString();
            var tags = await tagTable.GetEntity<ArticleTags>(stringId, stringId);
            return (tags == null) ? new ArticleTags(EntityId) : tags;
        }

        public async Task<IArticleTags> Update(IArticleTags Entity)
        {
            var inserted = new ArticleTags(Entity);
            await tagTable.ReplaceEntity(inserted);
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
