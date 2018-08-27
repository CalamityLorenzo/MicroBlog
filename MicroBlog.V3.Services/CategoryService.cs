using AzureStorage.V2.Helpers;
using AzureStorage.V2.Helpers.Context;
using AzureStorage.V2.Helpers.SimpleStorage;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services.BaseServices;
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
    public class CategoryService : BaseCategoryService
    {
        public CategoryService(CloudStorageContext cscCtx, MicroBlogOptions opts, ILogger logger, string tableName, string queueName) : base(cscCtx, opts, logger, tableName, queueName)
        {
        }
    }
}
