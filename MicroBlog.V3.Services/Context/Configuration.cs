using System;

namespace MicroBlog.V3.Services.Context
{
    public class MicroBlogConfiguration
    {
        public class Options
        {
            public Options() { }
            public Options(string StorageAccount, string ArticleBlob, string ArticleDetails, string TagQ, string TagsTable)
            {
                this.StorageAccount = StorageAccount;
                this.ArticleBlob = ArticleBlob;
                this.ArticleDetails = ArticleDetails;
                this.TagQ = TagQ;
                this.TagsTable = TagsTable;
            }

            public string StorageAccount { get; internal set; }
            public string ArticleBlob { get; internal set; }
            public string ArticleDetails { get; internal set; }
            public string TagQ { get; internal set; }
            public string TagsTable { get; internal set; }
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
