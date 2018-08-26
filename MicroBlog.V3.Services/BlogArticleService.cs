using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Entities.Models;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Logging;
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

        public BlogArticleService (CloudStorageContext ctx, MicroBlogOptions opts, ILogger logger)
        {
            this.ctx = ctx;
            this.opts = opts;
            this.logger = logger;
        }

        async Task<ICompletePost> Add(ICompletePost post)
        {
            // In the we deconstruct the original message, to easier to manage parts
            (var article, var tags, var categories) = new CompleteBlogEntry(post);
            var articleService = new ArticleService(ctx, opts);
            var tagService = new TagService(ctx, opts);
            // INsert and create a new Blog Article
            var entity = await articleService.Create(article);
            // Ditto the tsg, and categories
            var createdTags = await tagService.Create(tags.Tags.ToList(), entity.Id);
            var addedEntry = new CompleteBlogEntry(
                                    await articleService.Get(entity.Id), 
                                    createdTags,
                                    new ArticleCategories(categories.Tags, entity.Id));
            return addedEntry;
        }

        async Task<ICompletePost> Update(ICompletePost post)
        {
            (var article, var tags, var categories) = new CompleteBlogEntry(post);
            var articleService = new ArticleService(ctx, opts);
            var tagService = new TagService(ctx, opts);
            var categoriesService = new CategoryService(ctx, opts);
            // INsert and create a new Blog Article
            var updatedArticle = await articleService.Update(article);

            var updateCategories = await categoriesService.Update(categories);
            // Ditto the tsg, and categories
            var updatedTags = await tagService.Update(tags);
            var updatedEntry = new CompleteBlogEntry(
                                    updatedArticle,
                                    updatedTags,
                                    updateCategories);
            return updatedEntry;
        }
        async Task<bool> Retract(int PostId)
        {
            throw new ArgumentOutOfRangeException();
        }
        async Task<ICompletePost> Get(Guid PostId)
        {
            var articleService = ArticleService.GetManager();
            var tagService = new TagService(ctx, opts);

            var post = await articleService.Get(PostId);
            var tags = await tagService.Get(PostId);

            return new CompleteBlogEntry(post, tags, new ArticleCategories());

        }
        async Task<ICompletePost> Get(string Url)
        {
            throw new ArgumentException();
        }
    }
}
