using MI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace MI.Extensions
{
    /// <summary>DataExtension</summary>
    public static class DataExtensions
    {
        #region HasValue
        /// <summary>判断列是否有值</summary>
        /// <param name="row">对象</param>
        /// <param name="columnName">列名</param>
        /// <returns>是否存在值</returns>
        public static bool HasValue(this DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
                return false;

            return !row.IsNull(columnName);
        }

        /// <summary>判断列是否有值</summary>
        /// <param name="row">对象</param>
        /// <param name="columnIndex">列号</param>
        /// <returns>是否存在值</returns>
        public static bool HasValue(this DataRow row, int columnIndex)
        {
            if (row == null || row.Table.Columns.Count < columnIndex)
                return false;

            return !row.IsNull(columnIndex);
        }
        #endregion

        #region GetValue
        /// <summary>获得值</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="row">对象</param>
        /// <param name="columnName">列名</param>
        /// <returns>值</returns>
        public static T GetValue<T>(this DataRow row, string columnName)
        {
            if (row == null || !row.HasValue(columnName))
                return default(T);

            return UniversalTypeConverter.TryConvert(row[columnName], typeof(T), out var value) ? (T)value : default(T);
        }

        /// <summary>获得值</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="row">对象</param>
        /// <param name="columnIndex">列名</param>
        /// <returns>值</returns>
        public static T GetValue<T>(this DataRow row, int columnIndex)
        {
            if (row == null || !row.HasValue(columnIndex))
                return default(T);

            return UniversalTypeConverter.TryConvert(row[columnIndex], typeof(T), out var value) ? (T)value : default(T);
        }

        /// <summary>获得值</summary>
        /// <param name="row">对象</param>
        /// <param name="columnName">列名</param>
        /// <returns>值</returns>
        public static string GetValue(this DataRow row, string columnName)
        {
            if (row == null || !row.HasValue(columnName))
                return null;

            return row[columnName].ToString();
        }

        /// <summary>获得值</summary>
        /// <param name="row">对象</param>
        /// <param name="columnIndex">列号</param>
        /// <returns>值</returns>
        public static string GetValue(this DataRow row, int columnIndex)
        {
            if (row == null || !row.HasValue(columnIndex))
                return null;

            return row[columnIndex].ToString();
        }
        #endregion

        #region ToDictionary
        /// <summary>将行转换成字典</summary>
        public static IReadOnlyDictionary<string, object> ToDictionary(this DataRow row)
        {
            if (row == null)
                return new Dictionary<string, object>();

            var dic = new Dictionary<string, object>();
            foreach (DataColumn column in row.Table.Columns)
            {
                dic[column.ColumnName] = row[column.ColumnName];
            }
            return dic;
        }

        /// <summary>将行转换成字典列表</summary>
        public static IEnumerable<IReadOnlyDictionary<string, object>> ToDictionary(this IEnumerable<DataRow> rows)
        {
            if (rows == null)
                return new Dictionary<string, object>[0];

            return rows.Select(ToDictionary);
        }

        /// <summary>将表转换成字典列表</summary>
        public static IEnumerable<IReadOnlyDictionary<string, object>> ToDictionary(this DataTable dt) =>
            dt == null || dt.Rows.Count == 0 ? new IReadOnlyDictionary<string, object>[0] : dt.Rows.Cast<DataRow>().Select(ToDictionary);

        /// <summary>将表第一列做Key、第二列做Value组成字典</summary>
        public static IReadOnlyDictionary<TK, TV> ToDictionary<TK, TV>(this DataTable dt) => ToDictionary<TK, TV>(dt, 0, 1);

        /// <summary>将表指定的两列组成字典</summary>
        public static IReadOnlyDictionary<TK, TV> ToDictionary<TK, TV>(this DataTable dt, int keyIndex, int valueIndex)
        {
            var result = new Dictionary<TK, TV>();

            if (dt == null || dt.Rows.Count == 0)
                return result;

            foreach (DataRow row in dt.Rows)
            {
                result[row.GetValue<TK>(keyIndex)] = row.GetValue<TV>(valueIndex);
            }

            return result;
        }

        /// <summary>将表指定的两列组成字典</summary>
        public static IReadOnlyDictionary<TK, TV> ToDictionary<TK, TV>(this DataTable dt, string keyColumn, string valueColumn)
        {
            var result = new Dictionary<TK, TV>();

            if (dt == null || dt.Rows.Count == 0)
                return result;

            foreach (DataRow row in dt.Rows)
            {
                var key = row.GetValue<TK>(keyColumn);
                if (key != null)
                    result[key] = row.GetValue<TV>(valueColumn);
            }

            return result;
        }
        #endregion

        #region ToList
        /// <summary>取出表第一列值</summary>
        public static IReadOnlyList<T> ToList<T>(this DataTable dt) => ToList<T>(dt, 0);

        /// <summary>取出表指定列值</summary>
        public static IReadOnlyList<T> ToList<T>(this DataTable dt, int columnIndex)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new T[0];

            var result = new T[dt.Rows.Count];

            for (var index = 0; index < result.Length; index++)
            {
                result[index] = dt.Rows[index].GetValue<T>(columnIndex);
            }

            return result;
        }

        /// <summary>取出表指定列值</summary>
        public static IReadOnlyList<T> ToList<T>(this DataTable dt, string columnName)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new T[0];

            var result = new T[dt.Rows.Count];

            for (var index = 0; index < result.Length; index++)
            {
                result[index] = dt.Rows[index].GetValue<T>(columnName);
            }

            return result;
        }
        #endregion

        #region DataRow ConvertTo
        public static T ConvertTo<T>(this DataRow row) where T : class, new() => row.ConvertTo(() => new T());

        public static T ConvertTo<T>(this DataRow row, Func<T> ctor) where T : class => row?.Bind(ctor(), typeof(T).GetPublicProperties());

        internal static T Bind<T>(this DataRow row, T obj, PropertyInfo[] properties) where T : class
        {
            if (obj is BaseModel)
                (obj as BaseModel).Parse(row, properties);
            else
                foreach (var property in properties)
                {
                    var attr = property.GetColumnAttribute();

                    foreach (var name in attr.Names)
                    {
                        if (row.HasValue(name) && attr.TryConvert(row[name], out var result))
                            try
                            {
                                property.FastSetValue(obj, result);

                                break;
                            }
                            catch { }
                    }
                }
            return obj;
        }
        #endregion

        #region DataTable ConvertTo
        /// <summary>Linq延迟执行</summary>
        public static IEnumerable<T> ConvertTo<T>(this DataTable dt) where T : class, new() => dt.ConvertTo(() => new T());

        /// <summary>Linq延迟执行</summary>
        public static IEnumerable<T> ConvertTo<T>(this DataTable dt, Func<T> ctor) where T : class
        {
            if (dt == null || dt.Rows.Count == 0)
                yield break;

            var properties = typeof(T).GetPublicProperties();
            var enumerator = dt.Rows.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return ((DataRow)enumerator.Current).Bind(ctor(), properties);
            }
        }
        #endregion

        #region IEnumerable DataRow ConvertTo
        /// <summary>Linq延迟执行</summary>
        public static IEnumerable<T> ConvertTo<T>(this IEnumerable<DataRow> rows) where T : class, new() => rows.ConvertTo(() => new T());

        /// <summary>Linq延迟执行</summary>
        public static IEnumerable<T> ConvertTo<T>(this IEnumerable<DataRow> rows, Func<T> ctor) where T : class, new()
        {
            if (rows == null)
                yield break;

            var properties = typeof(T).GetPublicProperties();
            using (var enumerator = rows.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current.Bind(ctor(), properties);
                }
            }
        }
        #endregion
    }
}
