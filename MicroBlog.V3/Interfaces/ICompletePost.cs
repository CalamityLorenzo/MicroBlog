using System;
using System.Collections.Generic;

namespace MicroBlog.V3.Interfaces
{
    // THis is what the front end sees
    // this get's decomposed to the relevant components.
    public interface ICompletePost
    {
        Guid Id { get; }
        string Url { get; }
        string Title { get; }
        string Article { get; }
        string Synopsis { get; }
        string Author { get; }
        IEnumerable<string> Tags { get; }
        IEnumerable<string> Categories { get; }
        DateTime Created { get; }
        DateTime? Published { get; }
    }
}
