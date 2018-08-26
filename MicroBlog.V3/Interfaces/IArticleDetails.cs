using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{
    public interface IArticleDetails 
    {
        Guid Id { get; }
        string Url { get; }
        string Title { get; }
        string Synopsis { get; }
        string Author { get; }
        DateTime Created { get; }
        DateTime? Published { get; }
    } 
}
