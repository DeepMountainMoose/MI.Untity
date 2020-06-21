using MI.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MI.Models
{
    /// <summary>模型基类</summary>
    [Serializable]
    public abstract class BaseModel
    {
        /// <summary>Parse</summary>
        protected internal virtual void Parse(DataRow row, PropertyInfo[] properties)
        {
            if (row == null)
                return;

            foreach (var property in properties)
            {
                var attr = property.GetColumnAttribute();

                foreach (var name in attr.Names)
                {
                    if (row.HasValue(name) && attr.TryConvert(row[name], out var result))
                        try
                        {
                            property.SetValue(this, result, null);

                            break;
                        }
                        catch { }
                }
            }
        }

        /// <summary>JSON</summary>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    /// <summary>添加额外的名称，属性名优先生效</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>ctor</summary>
        internal ColumnAttribute() { }

        /// <summary>ctor</summary>
        public ColumnAttribute(string alias, params string[] aliases)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException(null, nameof(alias));

            if (aliases == null || aliases.Length == 0)
                Aliases = new[] { alias };
            else
            {
                var names = aliases.ToList();
                names.Insert(0, alias);
                Aliases = names.ToArray();
            }
        }

        /// <summary>ctor</summary>
        public ColumnAttribute(Type type, params string[] aliases)
        {
            Type = type;
            Aliases = aliases;
        }

        /// <summary>属性名</summary>
        public string PropertyName { get; internal set; }

        /// <summary>别名</summary>
        public ICollection<string> Aliases { get; protected set; }

        /// <summary></summary>
        public Type Type { get; set; }

        /// <summary>PropertyName + Aliases</summary>
        public ICollection<string> Names
        {
            get
            {
                if (Aliases == null || Aliases.Count == 0)
                    return new[] { PropertyName };

                var names = Aliases.ToList();
                names.Insert(0, PropertyName);
                return names;
            }
        }

        public bool TryConvert(object value, out object result)
        {
            if (value == null || value == DBNull.Value)
            {
                result = Type.GetTypeInfo().IsValueType ? Activator.CreateInstance(Type) : null;
                return true;
            }

            return TryConvertCore(value, out result, CultureInfo.CurrentCulture);
        }

        protected virtual bool TryConvertCore(object value, out object result, CultureInfo culture) => UniversalTypeConverter.TryConvert(value, Type, out result, culture);
    }

    /// <summary>添加额外的名称，属性名优先生效</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CollectionColumnAttribute : ColumnAttribute
    {
        /// <summary>ctor</summary>
        public CollectionColumnAttribute(string[] separator, params string[] aliases)
        {
            Separator = separator;
            Aliases = aliases;
        }

        /// <summary>ctor</summary>
        public CollectionColumnAttribute(Type type, string[] separator, params string[] aliases)
        {
            Type = type;
            Separator = separator;
            Aliases = aliases;
        }

        /// <summary>默认[",", ";"]，只支持Array或IEnumerable&lt;&gt;</summary>
        public string[] Separator { get; set; }

        public bool IsCollection => Type.GetTypeInfo().GetInterfaces().Any(type => type == typeof(IEnumerable));

        protected override bool TryConvertCore(object value, out object result, CultureInfo culture)
        {
            result = null;

            var enumerable = value as IEnumerable;
            if (enumerable == null || !IsCollection)
                return false;

            var array = value is string ? enumerable.ToString().Split(Separator?.Length > 0 ? Separator : new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries) : enumerable.OfType<object>().ToArray();

            return UniversalTypeConverter.TryConvertCollection(array, Type, out result);
        }
    }

    /// <summary>添加额外的名称，属性名优先生效</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonColumnAttribute : ColumnAttribute
    {
        /// <summary>ctor</summary>
        public JsonColumnAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        /// <summary>ctor</summary>
        public JsonColumnAttribute(Type type, params string[] aliases)
        {
            Type = type;
            Aliases = aliases;
        }

        protected override bool TryConvertCore(object value, out object result, CultureInfo culture)
        {
            result = JsonConvert.DeserializeObject(value is string ? value.ToString() : JsonConvert.SerializeObject(value), Type);

            return true;
        }
    }
}
