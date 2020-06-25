using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     <see cref="IList{T}" />的扩展方法.
    /// </summary>
    internal static class ListExtensions
    {
        /// <summary>
        ///     拓扑排序,根据依赖性来对List序列进行排序
        /// </summary>
        /// <typeparam name="T">List所存在的类型.</typeparam>
        /// <param name="source">将要被排序的List</param>
        /// <param name="getDependencies">指定依赖性判断的方法</param>
        /// <returns></returns>
        public static List<T> SortByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">List所存在的类型.</typeparam>
        /// <param name="item">指定被检查的Item</param>
        /// <param name="getDependencies">指定依赖性判断的方法</param>
        /// <param name="sorted">存储排序后的List</param>
        /// <param name="visited">存储被访问过的item的Dictionary</param>
        private static void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted,
            Dictionary<T, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found!");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
