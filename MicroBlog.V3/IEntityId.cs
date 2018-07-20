using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Interfaces
{
    public interface IEntityId<T>
    {
        T Id { get; }
    }
}
