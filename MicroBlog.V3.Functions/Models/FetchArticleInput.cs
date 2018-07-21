using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Functions.Models
{
    public class FetchArticleInput
    {
        public FetchArticleInput()
        {
            Url = "";
        }

        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}
