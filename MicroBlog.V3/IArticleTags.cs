using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{
    public interface IArticleTags
    {
        Guid Id { get; }
        IEnumerable<string> Tags { get; }
    }
}
