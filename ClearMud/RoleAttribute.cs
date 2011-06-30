using System;

namespace ClearMudSharp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface,AllowMultiple = false,Inherited = false)]
    public class RoleAttribute : Attribute
    {
        public string Name { get; set; }
    }
}