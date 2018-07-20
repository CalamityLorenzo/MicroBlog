using System;
using System.Collections.Generic;

namespace MicroBlog.V3.Interfaces
{
    public interface IClientArticle 
    {
        Guid Id { get; }
        string Url { get; }
        string Title { get; }
        string Article { get; }
        string Synopsis { get; }
        string Author { get; }
        DateTime Created { get; }
        DateTime? Published { get; }
    }
}
