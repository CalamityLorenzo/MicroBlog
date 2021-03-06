﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{
    public interface IArticle
    {
        Guid Id { get;}
        string Title { get; }
        string ArticleText { get; }
    }
}
