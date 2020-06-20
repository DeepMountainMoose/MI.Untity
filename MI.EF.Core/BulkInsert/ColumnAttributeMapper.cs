using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MI.EF.Core.BulkInsert
{
    public class ColumnAttributeMapper<T>
    {
        public static Dictionary<string, Dictionary<string, string>> ColumnPropertyMapper = new Dictionary<string, Dictionary<string, string>>();

        public ColumnAttributeMapper()
        {
            if (!ColumnPropertyMapper.ContainsKey(typeof(T).Name))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    var attribute = prop.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();
                    if (attribute == null)
                    {
                        dict.Add(prop.Name, prop.Name);
                    }
                    else
                    {
                        dict.Add(prop.Name, attribute.Name);
                    }
                }

                ColumnPropertyMapper.Add(typeof(T).Name, dict);
            }
        }

        public static DataTable MapToDataTable(IEnumerable<T> entities)
        {
            var dataTable = new DataTable();
            var columnsDict = new Dictionary<string, object>();
            T t = (T)Activator.CreateInstance(typeof(T));

            dataTable.TableName = ((TableAttribute)typeof(T).GetCustomAttributes(true).FirstOrDefault()).Name;
            var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0) { return new DataTable(); }

            foreach(var property in properties)
            {
                if(ColumnPropertyMapper.ContainsKey(t.GetType().Name))
                {
                    var dict = ColumnPropertyMapper[t.GetType().Name];
                    var DBproperty = dict[property.Name];

                    PropertyInfo propertyInfo = t.GetType().GetProperty(property.Name);
                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    dataTable.Columns.Add(DBproperty, propertyType);
                    columnsDict.Add(DBproperty, null);
                }
            }

            foreach(var entity in entities)
            {
                foreach(var propertyInfo in properties)
                {
                    var propertyValue = propertyInfo.GetValue(entity);
                    string name = propertyInfo.Name;
                    if(ColumnPropertyMapper.ContainsKey(t.GetType().Name))
                    {
                        var dict = ColumnPropertyMapper[t.GetType().Name]; ;
                        var property = dict[name];
                        columnsDict[property] = propertyValue;
                    }
                }

                var record = columnsDict.Values.ToArray();
                dataTable.Rows.Add(record);
            }

            return dataTable;
        }
    }
}
