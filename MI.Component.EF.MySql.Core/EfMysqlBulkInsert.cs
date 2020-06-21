using MI.EF.Core.BulkInsert;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MI.Component.EF.MySql.Core
{
    public class EfMysqlBulkInsert
    {
        private static readonly int BatchSize = 2000;
        public EfMysqlBulkInsert()
        { }

        public int BulkInsertSql<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction)
        {
            var columnAttributeMapper = new ColumnAttributeMapper<TEntity>();
            try
            {
                int RecordsAffected = 0;
                var entils = entities.ToList();
                while (entils.Count > 0)
                {
                    RecordsAffected += MysqlInsertSql((MySqlTransaction)transaction, entities.Take(BatchSize));
                    if(entils.Count>BatchSize)
                    {
                        entils.RemoveRange(0, BatchSize);
                    }
                    else
                    {
                        entils.RemoveRange(0, entils.Count);
                    }
                }

                return RecordsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int MysqlInsertSql<T>(MySqlTransaction Tranconnection, IEnumerable<T> entities)
        {
            if(!entities.Any())
            {
                return 0;
            }

            T t = (T)Activator.CreateInstance(typeof(T));
            var entityName = ((TableAttribute)typeof(T).GetCustomAttributes(true).FirstOrDefault()).Name;

            var parameters = new List<MySqlParameter>();
            Func<string, int, string> getParameterName = (columnName, i) =>
            {
                return $"@{columnName}_{i}";
            };

            Func<bool, string> getFormatter = (isLast) =>
            {
                return isLast ? "{0}" : "{0},";
            };

            var sql = new StringBuilder();
            sql.Append($"INSERT INTO {entityName}(");
            var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0) { return 0; }
            var strColumns = new List<string>();
            foreach(var property in properties)
            {
                if (ColumnAttributeMapper<T>.ColumnPropertyMapper.ContainsKey(t.GetType().Name))
                {
                    var dict = ColumnAttributeMapper<T>.ColumnPropertyMapper[t.GetType().Name];
                    var DBproperty = dict[property.Name];
                    strColumns.Add(DBproperty);
                }
            }
            sql.Append(string.Join(",", strColumns));
            sql.Append(") VALUES");
            var rowCount = 0;
            foreach(var entity in entities)
            {
                var ColumnCount = 0;
                sql.Append("(");
                foreach (var propertyInfo in properties)
                {
                    rowCount++;
                    ColumnCount++;
                    var propertyValue = propertyInfo.GetValue(entity);
                    string name = propertyInfo.Name;
                    if (ColumnAttributeMapper<T>.ColumnPropertyMapper.ContainsKey(t.GetType().Name))
                    {
                        var dict = ColumnAttributeMapper<T>.ColumnPropertyMapper[t.GetType().Name];
                        string columnName = dict[name];
                        var parameterName = getParameterName(columnName, rowCount);
                        var parameterValue = propertyValue;
                        var formatter = getFormatter(ColumnCount == properties.Count());
                        sql.AppendFormat(formatter, parameterName);
                        parameters.Add(new MySqlParameter() { ParameterName = parameterName, Value = parameterValue });
                    }
                }
                sql.Append("),");
            }
            if (sql.Length > 0)
            {
                sql.Remove(sql.Length - 1, 1);
            }
            var command = new MySqlCommand()
            {
                Connection = Tranconnection.Connection,
                CommandTimeout = 30,
                CommandText = sql.ToString(),
                CommandType = CommandType.Text,
                Transaction = Tranconnection,
            };
            command.Parameters.AddRange(parameters.ToArray());
            var RecordsAffected = command.ExecuteNonQuery();
            return RecordsAffected;
        }
    }
}
