using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroBlog.V3.Interfaces
{
    public interface ITagService: IBasicRepo<IArticleTags>
    {
        Task<IArticleTags> Create(IEnumerable<string> tags, Guid Id);
    }

    public interface ICategoryService : IBasicRepo<IArticleCategories>
    {
        Task<IArticleCategories> Create(IEnumerable<string> tags, Guid Id);
    }
}
