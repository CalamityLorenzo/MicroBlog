using System;

namespace MicroBlog.V3.Services.Context
{
    public class MicroBlogConfiguration
    {
        public class Options
        {
            public Options() { }
            public Options(string StorageAccount, string ArticleBlob, string ArticleDetails, string TagQueue, string TagTable, string CategoryQueue, string CategoryTable)
            {
                this.StorageAccount = StorageAccount;
                this.ArticleBlob = ArticleBlob;
                this.ArticleDetails = ArticleDetails;
                this.TagQueue = TagQueue;
                this.TagTable = TagTable;
                this.CategoryQueue = CategoryQueue;
                this.CategoryTable = CategoryTable;
            }

            public string StorageAccount { get; internal set; }
            public string ArticleBlob { get; internal set; }
            public string ArticleDetails { get; internal set; }
            public string TagQueue { get; internal set; }
            public string TagTable { get; internal set; }
            public string CategoryQueue { get; internal set; }
            public string CategoryTable { get; internal set; }
        }


        private static Options _opts = new Options();

        public static void SetConfiguration(Func<Options> opts)
        {
            if (opts != null)
            {
                _opts = opts();
            }
        }

        internal static Options GetOptions() => _opts;
    }
}
