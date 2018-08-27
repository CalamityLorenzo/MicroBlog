using System;

namespace MicroBlog.V3.Entities.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CreateMatchMethodsAttribute : Attribute
    {
        public Type[] Types;

        public CreateMatchMethodsAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}