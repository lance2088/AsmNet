using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    public static class LazyMethod
    {
        public static void Lazy<T>(this T item, Action<T> work)
        {
            work(item);
        }
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute() { }
    }
}