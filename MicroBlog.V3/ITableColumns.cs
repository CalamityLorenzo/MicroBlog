using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{
    public interface ITableColumns
    {
        IEnumerable<string> SelectColumns { get; }
    }
}
