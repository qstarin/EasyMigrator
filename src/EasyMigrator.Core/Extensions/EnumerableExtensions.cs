using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Extensions
{
    static public class EnumerableExtensions
    {
        static public void ForEach<T>(this IEnumerable<T> items, Action<T> action) { foreach (var item in items) action(item); }
    }
}
