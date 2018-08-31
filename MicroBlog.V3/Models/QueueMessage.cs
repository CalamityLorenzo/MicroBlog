using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Entities.Models
{
    // these
    public class QueueMessage : IArticleQueueMessage
    {
        public QueueMessage() { }
        public QueueMessageStatus Status { get; set; }
        public Guid ArticleId { get; set; }

        public override string ToString()
        {
            return $"{ArticleId.ToString()} {Status}";
        }
    }
}
