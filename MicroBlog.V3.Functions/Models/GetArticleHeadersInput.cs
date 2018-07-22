using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Functions.Models
{
    public class GetArticleHeadersInput
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
