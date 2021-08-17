using AkaDB.MySql;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Dao
{
    public class Dao<T> :INotifyPropertyChanged where T : new()
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            });
        }


        public static async Task<List<T>> GetAllObjects(uint shardIndex)
        {
            return await GetObjects(shardIndex, null);
        }

        public static async Task<T> GetObject(uint shardIndex, string where)
        {
            var objects = await GetObjects(shardIndex, where);
            return objects.FirstOrDefault();
        }

        static async Task<List<T>> GetObjects(uint shardIndex, string where)
        {
            var result = new List<T>();

            var attributes = typeof(T).GetCustomAttributes(typeof(TableAttribute), false).Where(attibute => attibute is TableAttribute).Cast<TableAttribute>();
            if (attributes.Any())
            {
                var tableAttr = attributes.First();
                var query = $"SELECT {Columns()} FROM {tableAttr.DB}.{tableAttr.Name} ";
                if (where?.Length > 0)
                    query += where;
                query += ";";

                using (var db = new DBContext(shardIndex, tableAttr.DBContext))
                {
                    using (var cursor = await db.ExecuteReaderAsync(query))
                    {
                        while (true == cursor.Read())
                        {
                            result.Add(ObjectFromDBData(cursor));
                        }
                    }
                }
            }
            return result;
        }

        static string Columns()
        {
            var properties = typeof(T).GetProperties();
            return string.Join(",", properties.Where(property => property.PropertyType.IsValueType || property.PropertyType == typeof(string)).Select(property => property.Name));
        }

        static T ObjectFromDBData(DbDataReader cursor)
        {
            var properties = typeof(T).GetProperties();
            var obj = new T();

            foreach( var property in properties)
            {
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    try
                    {
                        var columnValue = cursor[property.Name];
                        property.SetValue(obj, columnValue);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return obj;
        }
    }
}
