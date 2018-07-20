using MicroBlog.V3.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Services.Models
{
    // these
    class QueueMessage : IArticleQueueMessage
    {
        public QueueMessage() { }
        public QueueMessageStatus Status { get; set; }
        public Guid ArticleId { get; set; }
    }
}
