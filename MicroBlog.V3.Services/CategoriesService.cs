using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using MicroBlog.V3.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MicroBlog.V3.Services.Context.CloudStorageContext;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;

namespace MicroBlog.V3.Services
{
    public class CategoryService : ICategoryService
    {
        private SimpleTableHelper tagTable;
        private SimpleQueueHelper tagQueue;
        private CloudStorageContext cscCtx;

        internal CategoryService(CloudStorageContext cscCtx, Options opts)
        {
            tagTable = cscCtx.CreateTableHelper(opts.CategoryTable);
            tagQueue = cscCtx.CreateQueueHelper(opts.CategoryQueue);
            this.cscCtx = cscCtx;
        }

        public Task<IArticleCategories> Create(IEnumerable<string> tags, Guid Id)
        {
            return this.Create(new ArticleCategories(tags, Id));
        }

        public async Task<IArticleCategories> Create(IArticleCategories Entity)
        {
            await tagTable.InsertToTable(new ArticleCategories(Entity));
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
            var stringId = EntityId.ToString();
            var tag = await tagTable.GetEntity<ArticleTags>(stringId, stringId);
            await tagTable.DeleteEntity(tag);
            // update statistics.
            await tagQueue.InsertIntoQueue(JsonConvert.SerializeObject(new QueueMessage { ArticleId = EntityId, Status = QueueMessageStatus.Deleted }));
        }

        public async Task<IArticleCategories> Get(Guid EntityId)
        {
            var stringId = EntityId.ToString();
            return await tagTable.GetEntity<ArticleCategories>(stringId, stringId);
        }

        public async Task<IArticleCategories> Update(IArticleCategories Entity)
        {
            var inserted = new ArticleCategories(Entity);
            await tagTable.ReplaceEntity(inserted);
            return inserted;
        }

        public static ICategoryService GetManager()
        {
            var opts = MicroBlogConfiguration.GetOptions();
            return new CategoryService(new CloudStorageContext(opts.StorageAccount), opts);
        }
    }
}
