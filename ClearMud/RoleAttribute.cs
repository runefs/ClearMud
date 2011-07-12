/* Copyright 2011 by Rune Funch Søltoft
 * the code my not be used commercially
 * and when used in any form this copyright notice shall
 * be included in all parts that uses this code
 */
using System;

namespace ClearMudSharp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface,AllowMultiple = false,Inherited = false)]
    public class RoleAttribute : Attribute
    {
        public string Name { get; set; }
    }
}