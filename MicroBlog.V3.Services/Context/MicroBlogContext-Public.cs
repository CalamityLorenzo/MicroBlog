using MicroBlog.V3.Interfaces;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroBlog.V3.Services.Context
{
    partial class MicroBlogContext
    {
        public async Task AddArticleToStorage(string serializedArticle, string fileName)
        {
            var NewArticleStorage = await this.GetBlobContainer(this._newArticleBlobStorage);
            await NewArticleStorage.CreateIfNotExistsAsync();
            var NewArticleFile = NewArticleStorage.GetBlockBlobReference(fileName);
            await NewArticleFile.DeleteIfExistsAsync();
            await NewArticleFile.UploadTextAsync(serializedArticle);

        }

        public async Task TagsProcessorQueueMessage(IArticleQueueMessage msg)
        {
            var tagProcQ = await this.GetQueue(this._tagProcessorQ);
            await tagProcQ.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(msg)));
        }
    }
}
