using AzureStorage.V2.Helpers.Context;
using MicroBlog.V3.Entities.Models;
using MicroBlog.V3.Interfaces;
using MicroBlog.V3.Services;
using MicroBlog.V3.Services.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBlogv3.Models;

namespace TestBlogv3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IConfigurationBuilder build = new ConfigurationBuilder()
                         .SetBasePath(Environment.CurrentDirectory)
                         .AddJsonFile("local.settings.json ", false, true);
            IConfigurationRoot config = build.Build();
            IConfigurationSection appKeys = config.GetSection("appSettings");
            ILoggerFactory loggerFactory = new LoggerFactory()
                                        .AddConsole(LogLevel.Trace)
                                        .AddDebug();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            DoThings(appKeys, logger).Wait();
        }

        private static async Task DoThings(IConfigurationSection appKeys, ILogger logger)
        {
            MicroBlogConfiguration.SetConfiguration(() => new MicroBlogConfiguration.MicroBlogOptions(appKeys["StorageAcc"], appKeys["NewArticleStore"], appKeys["NewArticleDetails"], appKeys["TagProcessing"], appKeys["TagStore"], appKeys["catQueue"], appKeys["catStore"]));
            var allOpts = MicroBlogConfiguration.GetOptions();
            var cloudAcct = new CloudStorageContext(allOpts.StorageAccount);

            //await Stage1(cloudAcct, allOpts, logger);
            await Stage2(cloudAcct, allOpts, logger);

        }

        private static async Task Stage1(CloudStorageContext cloudAcct, MicroBlogConfiguration.MicroBlogOptions allOpts, ILogger logger)
        {
            var s = "Welcome to Stage1";
            logger.LogInformation(s);
            
            var blogOh1 = new CompleteBlogEntry("my-new-url", "The first Title I Choosed",
                                              "On top of the old help finishes an adult handicap. When can the drivel chew? How does the senior priest do the skip? Why can't a backlog pile a concentrate? The saga wins the proprietary equilibrium. The arrogance sponsors the jazz.", "article",
                                              "Paul lawrence",
                                              new List<string> { "KEllogs", "Tonka", "Roos" },
                                              new List<string> { "New-Science", "Killer-Bees", "Rune Doogle" },
                                              DateTime.Now, DateTime.Today.AddDays(-100));

            // In a whorld where this is a functionapp.
            // we pass this entire BlogPost into  function
            BlogArticleService bas = new BlogArticleService(cloudAcct, allOpts, logger);
            var newPost = await CreatePost(bas, blogOh1);

            var editedPost = new CompleteBlogEntry(newPost).WithTitle("Jelly bean fiend");

            var ratherEditedPost = await UpdatePost(bas, editedPost);
            logger.LogInformation($"{ratherEditedPost.Id} {ratherEditedPost.Url}");
            var furtherEditedPost = await UpdatePost(bas, ratherEditedPost);
            logger.LogInformation($"{furtherEditedPost.Id} {furtherEditedPost.Url}");
        }

        private static async Task Stage2(CloudStorageContext cloudAcct, MicroBlogConfiguration.MicroBlogOptions allOpts, ILogger logger)
        {

            var s = "Welcome to Stage2";
            logger.LogInformation(s);

            var blogOh2 = new CompleteBlogEntry("Further-Missions-OfMercy", "Another Blog Post I created",
                                              "Behaviour we improving at something to. Evil true high lady roof men had open. To projection considered it precaution an melancholy or. Wound young you thing worse along being ham. Dissimilar of favourable solicitude if sympathize middletons at. Forfeited up if disposing perfectly in an eagerness perceived necessary. Belonging sir curiosity discovery extremity yet forfeited prevailed own off. Travelling by introduced of mr terminated. Knew as miss my high hope quit. In curiosity shameless dependent knowledge up. ",
                                              @"Behaviour we improving at something to. Evil true high lady roof men had open. To projection considered it precaution an melancholy or. Wound young you thing worse along being ham. Dissimilar of favourable solicitude if sympathize middletons at. Forfeited up if disposing perfectly in an eagerness perceived necessary. Belonging sir curiosity discovery extremity yet forfeited prevailed own off. Travelling by introduced of mr terminated. Knew as miss my high hope quit. In curiosity shameless dependent knowledge up. 

Remember outweigh do he desirous no cheerful. Do of doors water ye guest. We if prosperous comparison middletons at. Park we in lose like at no. An so to preferred convinced distrusts he determine. In musical me my placing clothes comfort pleased hearing. Any residence you satisfied and rapturous certainty two. Procured outweigh as outlived so so. On in bringing graceful proposal blessing of marriage outlived. Son rent face our loud near. 

Do commanded an shameless we disposing do. Indulgence ten remarkably nor are impression out. Power is lived means oh every in we quiet. Remainder provision an in intention. Saw supported too joy promotion engrossed propriety. Me till like it sure no sons. 

You vexed shy mirth now noise. Talked him people valley add use her depend letter. Allowance too applauded now way something recommend. Mrs age men and trees jokes fancy. Gay pretended engrossed eagerness continued ten. Admitting day him contained unfeeling attention mrs out. 

Domestic confined any but son bachelor advanced remember. How proceed offered her offence shy forming. Returned peculiar pleasant but appetite differed she. Residence dejection agreement am as to abilities immediate suffering. Ye am depending propriety sweetness distrusts belonging collected. Smiling mention he in thought equally musical. Wisdom new and valley answer. Contented it so is discourse recommend. Man its upon him call mile. An pasture he himself believe ferrars besides cottage. 

Do play they miss give so up. Words to up style of since world. We leaf to snug on no need. Way own uncommonly travelling now acceptance bed compliment solicitude. Dissimilar admiration so terminated no in contrasted it. Advantages entreaties mr he apartments do. Limits far yet turned highly repair parish talked six. Draw fond rank form nor the day eat. 

Passage its ten led hearted removal cordial. Preference any astonished unreserved mrs. Prosperous understood middletons in conviction an uncommonly do. Supposing so be resolving breakfast am or perfectly. Is drew am hill from mr. Valley by oh twenty direct me so. Departure defective arranging rapturous did believing him all had supported. Family months lasted simple set nature vulgar him. Picture for attempt joy excited ten carried manners talking how. Suspicion neglected he resolving agreement perceived at an. 

Rooms oh fully taken by worse do. Points afraid but may end law lasted. Was out laughter raptures returned outweigh. Luckily cheered colonel me do we attacks on highest enabled. Tried law yet style child. Bore of true of no be deal. Frequently sufficient in be unaffected. The furnished she concluded depending procuring concealed. 

Agreed joy vanity regret met may ladies oppose who. Mile fail as left as hard eyes. Meet made call in mean four year it to. Prospect so branched wondered sensible of up. For gay consisted resolving pronounce sportsman saw discovery not. Northward or household as conveying we earnestly believing. No in up contrasted discretion inhabiting excellence. Entreaties we collecting unpleasant at everything conviction. 

It real sent your at. Amounted all shy set why followed declared. Repeated of endeavor mr position kindness offering ignorant so up. Simplicity are melancholy preference considered saw companions. Disposal on outweigh do speedily in on. Him ham although thoughts entirely drawings. Acceptance unreserved old admiration projection nay yet him. Lasted am so before on esteem vanity oh. 

",
                                              "Paul lawrence",
                                              new List<string> { "tag01", "Fag02", "Snag-3" },
                                              new List<string> { "People", "Flowers", "Dept of rage" },
                                              DateTime.Now, DateTime.Today.AddDays(-100));

            // In a whorld where this is a functionapp.
            // we pass this entire BlogPost into  function
            BlogArticleService bas = new BlogArticleService(cloudAcct, allOpts, logger);
            var newPost = await CreatePost(bas, blogOh2);

            var editedPost = new CompleteBlogEntry(newPost).WithTags(new List<string> { "Jelly", "Future", "Pillow" });

            var ratherEditedPost = new CompleteBlogEntry(await UpdatePost(bas, editedPost));
            logger.LogInformation($"{ratherEditedPost.Id} {ratherEditedPost.Url}");
            var editedUrl = ratherEditedPost.WithUrl("Slander-on-my-harmer");
            var furtherEditedPost = await UpdatePost(bas, editedUrl);
            logger.LogInformation($"{furtherEditedPost.Id} {furtherEditedPost.Url}");
        }

        private static async Task<ICompletePost> UpdatePost(BlogArticleService bas, ICompletePost blogPost)
        {

            return await bas.Update(blogPost);
        }

        private static async Task<ICompletePost> CreatePost(BlogArticleService bas, CompleteBlogEntry blogOh1)
        {

           return await bas.Add(blogOh1);
        }
    }
}
