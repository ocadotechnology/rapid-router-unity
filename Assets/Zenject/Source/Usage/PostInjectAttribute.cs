using System;
using ModestTree.Util;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PostInjectAttribute : PreserveAttribute
    {
    }
}
