using System;
using System.Collections.Generic;

namespace MicroBlog.V3.Services.Context
{
    public class MicroBlogConfiguration
    {
        public enum StorageList
        {
            Unknown = 0,
            ArticleBlob,
            ArticleDetails,
            TagQueue,
            TagTable,
            CategoryQueue,
            CategoryTable
        }

        public class MicroBlogOptions
        {
            public MicroBlogOptions() { }
            public MicroBlogOptions(string StorageAccount, string ArticleBlob, string ArticleDetails, string TagQueue, string TagTable, string CategoryQueue, string CategoryTable)
            {
                this.StorageAccount = StorageAccount;
                this.StorageNames = new Dictionary<string, string>();
                StorageNames.Add(StorageList.ArticleBlob.ToString(), ArticleBlob);

                StorageNames.Add(StorageList.ArticleDetails.ToString(), ArticleDetails);
                StorageNames.Add(StorageList.TagQueue.ToString(), TagQueue);
                StorageNames.Add(StorageList.TagTable.ToString(), TagTable);
                StorageNames.Add(StorageList.CategoryQueue.ToString(), CategoryQueue);
                StorageNames.Add(StorageList.CategoryTable.ToString(), CategoryTable);
            }

            public string this[StorageList tableName]
            {
                get
                {
                    return this.StorageNames.TryGetValue(tableName.ToString(), out var tblName) ? tblName : "";
                }
            }

            public string StorageAccount { get; set; }
            internal Dictionary<string, string> StorageNames { get; private set; }

        }

        private static MicroBlogOptions _opts = new MicroBlogOptions();

        public static void SetConfiguration(Func<MicroBlogOptions> opts)
        {
            if (opts != null)
            {
                _opts = opts();
            }
        }

        public static MicroBlogOptions GetOptions() => _opts;
    }
}
