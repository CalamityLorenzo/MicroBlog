using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{

    public enum QueueMessageStatus
    {
        Unknown = 0,
        Added = 10,
        Deleted = 20,
        Updated = 30
    }

    public interface IArticleQueueMessage
    {
        QueueMessageStatus Status { get; }
        Guid ArticleId { get; }
    }
}
