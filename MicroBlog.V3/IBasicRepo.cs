using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroBlog.V3.Interfaces
{
    public interface IBasicRepo<T>
    {
        Task<T> Get(Guid EntityId);
        Task<T> Create(T Entity);
        Task<T> Update(T Entity);
        Task Delete(T Entity);
        Task Delete(Guid Entity);
    }
}
