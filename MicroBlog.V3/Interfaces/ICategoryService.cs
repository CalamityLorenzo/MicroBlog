using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroBlog.V3.Interfaces
{
    public interface ICategoryService : IBasicRepository<IArticleCategories>
    {
        Task<IArticleCategories> Create(IEnumerable<string> tags, Guid Id);
    }
}
