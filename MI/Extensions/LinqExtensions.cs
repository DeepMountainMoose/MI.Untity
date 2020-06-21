using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Extensions
{
    public static class LinqExtensions
    {
        public static class Self<T>
        {
            public static readonly Func<T, T> Func = _ => _;
        }

        /// <summary>Enumerates the sequence and invokes the given action for each value in the sequence.</summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="onNext">Action to invoke for each element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> onNext)
        {
            if (source is List<T> list)
                list.ForEach(onNext);
            else if (source is T[] array)
                Array.ForEach(array, onNext);
            else
                foreach (var item in source)
                    onNext(item);
        }

        /// <summary>Enumerates the sequence and invokes the given action for each value in the sequence.</summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="onNext">Action to invoke for each element.</param>
        public static void ForEach<T>(this IEnumerable<T> source,Action<T,int> onNext)
        {
            var index = 0;
            foreach (var item in source)
                onNext(item, index++);
        }

        /// <summary>Enumerates the sequence and invokes the given action for each value in the sequence.</summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="onNext">Action to invoke for each element.</param>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> onNext)
        {
            foreach (var item in source)
                await onNext(item).ConfigureAwait(false);
        }

        /// <summary>Enumerates the sequence and invokes the given action for each value in the sequence.</summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="onNext">Action to invoke for each element.</param>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, int, Task> onNext)
        {
            var index = 0;
            foreach (var item in source)
                await onNext(item, index++).ConfigureAwait(false);
        }

        /// <summary>Where item not null.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class => source.Where(_ => _ != null);

        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(Self<IEnumerable<T>>.Func);

        #region Sort
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source) => source.OrderBy(Self<T>.Func);

        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.OrderBy(Self<T>.Func, comparer);

        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source) => source.OrderByDescending(Self<T>.Func);

        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.OrderByDescending(Self<T>.Func, comparer);
        #endregion

    }
}
