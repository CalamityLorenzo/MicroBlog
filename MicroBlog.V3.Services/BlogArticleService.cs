using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Entities.Models;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroBlog.V3.Services.Context.MicroBlogConfiguration;

namespace MicroBlog.V3.Services
{
    public class BlogArticleService
    {
        private readonly CloudStorageContext ctx;
        private readonly MicroBlogOptions opts;
        private readonly ILogger logger;

        public BlogArticleService(CloudStorageContext ctx, MicroBlogOptions opts, ILogger logger)
        {
            this.ctx = ctx;
            this.opts = opts;
            this.logger = logger;
        }

        public async Task<ICompletePost> Add(ICompletePost post)
        {
            // In the we deconstruct the original message, to easier to manage parts
           logger.LogInformation($"Creating new blog article {post.Title}");
            (var article, var tags, var categories) = new CompleteBlogEntry(post);
            var articleService = new ArticleService(ctx, opts, logger);
            var tagService = new TagService(ctx, opts, logger, opts[StorageList.TagTable], opts[StorageList.TagQueue]);
            var categoryService = new CategoryService(ctx, opts, logger, opts[StorageList.CategoryTable], opts[StorageList.CategoryQueue]);

            // INsert and create a new Blog Article
            var entity = await articleService.Create(article);
            // Ditto the tsg, and categories
            var createdTags = await tagService.Create(tags.Tags.ToList(), entity.Id);
            var createdCats = await categoryService.Create(categories.Tags.ToList(), entity.Id);
            var addedEntry = new CompleteBlogEntry(
                                    await articleService.Get(entity.Id),
                                    createdTags,
                                    createdCats);
            return addedEntry;
        }

        public async Task<ICompletePost> Update(ICompletePost post)
        {
            try
            {
                logger.LogInformation($"Update blog article {post.Id}");

                (var article, var tags, var categories) = new CompleteBlogEntry(post);
                // Services
                var articleService = new ArticleService(ctx, opts, logger);
                var tagService = new TagService(ctx, opts, logger, opts[StorageList.TagTable], opts[StorageList.TagQueue]);
                var categoryService = new CategoryService(ctx, opts, logger, opts[StorageList.CategoryTable], opts[StorageList.CategoryQueue]);

                // Set up the main 'document'
                var updatedArticle = await articleService.Update(article);
                // We changed the primaryKey (Guid, Url) so the url.
                // so We have to delete the existing tags, and categories
                if (post.Id != updatedArticle.Id)
                {
                    // Delete the ones with the original keys
                    await tagService.Delete(tags);
                    await categoryService.Delete(categories);
                    // Create a complete blogpost correct keys
                    var recreatedEntry = new CompleteBlogEntry(updatedArticle, tags, categories);
                    (_, var updatedTags, var updatedCategories) = recreatedEntry;
                    // create them as if it were a new post....Which it is 
                    await tagService.Create(tags);
                    await categoryService.Create(categories);
                    return recreatedEntry;
                }
                else
                {
                   // normal updates of thins
                    var updatedTags = await tagService.Update(tags);
                    var updateCategories = await categoryService.Update(categories);

                    return  new CompleteBlogEntry(
                                            updatedArticle,
                                            updatedTags,
                                            updateCategories);
                }
            }
            catch (StorageException ex)
            {
                logger.LogDebug(ex.Message, ex.StackTrace, "BlogArticleService.Update");
                throw;
            }
        }

        public async Task<ICompletePost> Get(Guid Id)
        {
            logger.LogInformation($"Get Post by Id:  {Id}");
            var articleService = new ArticleService(ctx, opts, logger);
            var tagService = new TagService(ctx, opts, logger, opts[StorageList.TagTable], opts[StorageList.TagQueue]);
            var categoryService = new CategoryService(ctx, opts, logger, opts[StorageList.CategoryTable], opts[StorageList.CategoryQueue]);

            var post = await articleService.Get(Id);
            var tags = await tagService.Get(Id);
            var categories = await categoryService.Get(Id);

            return new CompleteBlogEntry(post, tags, categories);
        }

        public async Task<ICompletePost> Get(string Url)
        {
            logger.LogInformation($"Get Post by Url:  {Url}");
            var articleService = new ArticleService(ctx, opts, logger);

            var tagService = new TagService(ctx, opts, logger, opts[StorageList.TagTable], opts[StorageList.TagQueue]);
            var categoryService = new CategoryService(ctx, opts, logger, opts[StorageList.CategoryTable], opts[StorageList.CategoryQueue]);

            var post = await articleService.GetByUrl(Url);
            var tags = await tagService.Get(post.Id);
            var cats = await categoryService.Get(post.Id);

            return new CompleteBlogEntry(post, tags, cats);
        }

        public async Task<bool> Retract(Guid PostId)
        {
            var articleService = new ArticleService(ctx, opts, logger);
            var post = new ClientArticle(await articleService.Get(PostId));
            // post.Published = null;
            var updated = post.WithPublished(null);
            await articleService.Update(updated);
            return true;

        }

        public Task<IEnumerable<IArticleDetails>> FindArticlDetails(DateTime start, DateTime end, int take, int skip)
        {
            var articleService = new ArticleService(ctx, opts, logger);
            return articleService.FindArticlDetails(start, end, take, skip);
        }
    }
}
