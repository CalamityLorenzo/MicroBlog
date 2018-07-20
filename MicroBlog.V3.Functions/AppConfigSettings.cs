using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace MicroBlog.V3.Functions
{
    internal class AppConfigSettings
    {
        public static Dictionary<string, string> GetSettings(string rootFolder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(rootFolder)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables();
            return builder.Build().GetSection("Values").AsEnumerable().ToDictionary(k => k.Key, v => v.Value);
        }

        public static void ConfigureBlogOptions(string rootFolder)
        {
            Dictionary<string, string> options = GetSettings(rootFolder);
            MicroBlogConfiguration.SetConfiguration(() =>
                    new MicroBlogConfiguration.Options(
                                options["Values:AzureWebJobsStorage"],
                                options["Values:ArticleBlob"],
                                options["Values:ArticleDetails"],
                                options["Values:TagQueue"],
                                options["Values:TagsTable"],
                                options["Values:CategoryQueue"],
                                options["Values:CategoryTable"]                                
                                ));

        }
    }
}
