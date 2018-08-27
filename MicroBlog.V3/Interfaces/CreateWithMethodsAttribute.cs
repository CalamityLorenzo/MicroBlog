using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBlog.V3.Entities.Interfaces
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CreateWithMethodsAttribute : Attribute
    {
        public Type[] Types;
        public CreateWithMethodsAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
