using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{
    public interface IArticleCategories
    {
        Guid Id { get; }
        IEnumerable<string> Tags { get; }
    }
}
