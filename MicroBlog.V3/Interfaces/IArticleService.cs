﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroBlog.V3.Interfaces
{
    public interface IArticleService : IBasicRepository<IClientArticle> {
        Task<IClientArticle> GetByUrl(string url);
        Task<IEnumerable<IArticleDetails>> FindArticlDetails(DateTime start, DateTime end, int take, int skip);
    }
}
