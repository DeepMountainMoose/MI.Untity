using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using MI.Models;

// ReSharper disable once CheckNamespace
namespace MI.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<Type, object> Cache = new ConcurrentDictionary<Type, object>();

        internal static ColumnAttribute GetColumnAttribute(this PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<ColumnAttribute>() ?? new ColumnAttribute();

            attr.PropertyName = property.Name;

            attr.Type = attr.Type ?? property.PropertyType;

            return attr;
        }

        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        internal static PropertyInfo[] GetPublicProperties(this Type type) => TypePropertiesCache.GetOrAdd(type, _ => _.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty));

        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> PropertySetterCache = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();

        internal static void FastSetValue(this PropertyInfo property, object instance, object value) => PropertySetterCache.GetOrAdd(property, p => new PropertyReflector(p).CreateSetter())(instance, value);

        /// <summary>将对象（公有属性）转换成字典，如果T继承了IReadOnlyDictionary&lt;string, object&gt;或者IDictionary&lt;string, object&gt;则返回对象字典本身</summary>
        public static IReadOnlyDictionary<string, object> ToDictionary<T>([CanBeNull] T obj) where T : class
        {
            switch (obj)
            {
                case null:
                    return null;
                case IReadOnlyDictionary<string, object> rd:
                    return rd;
                case IDictionary<string, object> d:
                    return new ReadOnlyDictionary<string, object>(d);
                default:
                    return ((Func<T, Dictionary<string, object>>) Cache.GetOrAdd(typeof(T), _ =>
                    {
                        var param = Expression.Parameter(typeof(T), "obj");

                        return Expression.Lambda<Func<T, Dictionary<string, object>>>(Expression.ListInit(
                                Expression.New(typeof(Dictionary<string, object>).GetConstructor(new Type[0]) ?? throw new InvalidOperationException()),
                                typeof(T).GetProperties()
                                    .Select(property => Expression.ElementInit(
                                        typeof(Dictionary<string, object>).GetMethod("Add") ?? throw new InvalidOperationException(),
                                        Expression.Constant(property.Name),
                                        property.PropertyType.IsValueType
                                            ? Expression.Convert(Expression.Property(param, property), typeof(object))
                                            : (Expression) Expression.Property(param, property)))),
                            param).Compile();
                    }))(obj);
            }
        }
    }

    public static class ObjectMapper
    {
        public static TResult ConvertTo<TSource, TResult>(TSource source)
            where TSource : class
            where TResult : class, new() => ConvertTo(source, () => new TResult());

        public static TResult ConvertTo<TSource, TResult>(TSource source, Func<TResult> ctor)
            where TSource : class
            where TResult : class => source?.Bind(ctor(), source.GetType().GetPublicProperties(), typeof(TResult).GetPublicProperties());

        public static IEnumerable<TResult> ConvertTo<TSource, TResult>(IEnumerable<TSource> sources)
            where TSource : class
            where TResult : class, new() => ConvertTo(sources, () => new TResult());

        public static IEnumerable<TResult> ConvertTo<TSource, TResult>(IEnumerable<TSource> sources, Func<TResult> ctor)
            where TSource : class
            where TResult : class
        {
            if (sources == null || !sources.Any())
                return new TResult[0];

            return new ObjectMapperEnumerable<TSource, TResult>(sources, ctor);
        }

        internal static TResult Bind<TResult>(this object source, TResult result, PropertyInfo[] sourceProperties, PropertyInfo[] resultProperties)
        {
            var sourceAttributes = sourceProperties.Select(property => Tuple.Create(property, property.GetColumnAttribute())).ToArray();
            foreach (var resultProperty in resultProperties)
            {
                var attr = resultProperty.GetColumnAttribute();

                var sourceProperty = sourceAttributes.FirstOrDefault(property => property.Item2.Names.Join(attr.Names, s => s, t => t, (s, t) => s, StringComparer.CurrentCultureIgnoreCase).Any());

                if (sourceProperty != null && attr.TryConvert(sourceProperty.Item1.GetValue(source, null), out var value))
                    try
                    {
                        resultProperty.SetValue(result, value, null);
                    }
                    catch { }
            }

            return result;
        }
    }

    internal class ObjectMapperEnumerable<TSource, TResult> : IEnumerable<TResult>, IEnumerable
            where TSource : class
            where TResult : class
    {
        private readonly IEnumerable<TSource> _sources;
        private readonly Func<TResult> _ctor;

        public ObjectMapperEnumerable(IEnumerable<TSource> sources, Func<TResult> ctor)
        {
            _sources = sources;
            _ctor = ctor;
        }

        public IEnumerator<TResult> GetEnumerator() => new ObjectMapperEnumerator<TSource, TResult>(_sources, _ctor);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class ObjectMapperEnumerator<TSource, TResult> : IEnumerator<TResult>
                where TSource : class
                where TResult : class
    {
        private readonly IEnumerator<TSource> _rowsEnumerator;
        private readonly Func<TResult> _ctor;
        private readonly Lazy<PropertyInfo[]> _sourceProperties;
        private readonly Lazy<PropertyInfo[]> _resultProperties;

        public ObjectMapperEnumerator(IEnumerable<TSource> sources, Func<TResult> ctor)
        {
            _rowsEnumerator = sources.GetEnumerator();
            _ctor = ctor;

            _sourceProperties = new Lazy<PropertyInfo[]>(() => typeof(TSource).GetPublicProperties());
            _resultProperties = new Lazy<PropertyInfo[]>(() => typeof(TResult).GetPublicProperties());
        }

        object IEnumerator.Current => Current;
        public TResult Current => _rowsEnumerator.Current.Bind(_ctor(), _sourceProperties.Value, _resultProperties.Value);

        public void Dispose()
        {
        }

        public bool MoveNext() => _rowsEnumerator.MoveNext();

        public void Reset()
        {
            _rowsEnumerator.Reset();
        }
    }

    internal class PropertyReflector
    {
        private readonly PropertyInfo _property;

        public PropertyReflector(PropertyInfo property)
        {
            _property = property;
        }

        public Action<object, object> CreateSetter()
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameter = Expression.Parameter(typeof(object), "parameter");
            var property = Expression.Property(Expression.Convert(instance, _property.DeclaringType), _property);
            var assign = Expression.Assign(property, Expression.Convert(parameter, _property.PropertyType));
            return Expression.Lambda<Action<object, object>>(assign, instance, parameter).Compile();
        }
    }
}
