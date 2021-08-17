using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using BattleLogic;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestHelper.BattleData
{
    public class TransactionManager : IDisposable
    {
        private struct Target
        {
            public string table;
            public string where;
            public Dictionary<string, string> updateColumns;
        }

        public DBContext DB => _db;
        DBContext _db = new DBContext();
        List<Target> _originUpdateDeleteSql = new List<Target>();
       
        public async Task AddUpdate(string table, string where, params (string column, string newVal)[] values)
        {
            var columns = values.Select(columnVal => columnVal.column);
            var vals = values.Select(columnVal => columnVal.newVal);
            var columnSql = string.Join(",", columns);

            using (var cursor = await _db.ExecuteReaderAsync($"SELECT {columnSql} from {table} WHERE {where};"))
            {
                while (cursor.Read())
                {
                    _originUpdateDeleteSql.Add(new Target { table = table, where = where, updateColumns = columns.ToDictionary(column => column, column => cursor[column].ToString()) } );
                    break;
                }
            }

            var updateSql = string.Join(",", columns.Zip(vals, (column, val) => $"{column} = '{val}' "));
            await _db.ExecuteNonQueryAsync($"UPDATE {table} SET {updateSql} WHERE {where};");
        }

        public async Task Insert(string table, string columns, string values, string deleteWhere)
        {
            await _db.ExecuteNonQueryAsync($"INSERT INTO {table} ( {columns} ) VALUES ( {values} ) ;");
            _originUpdateDeleteSql.Add(new Target { table = table, where = deleteWhere });            
        }

        public void Dispose()
        {
            try
            {
                foreach (var target in _originUpdateDeleteSql)
                {
                    if (target.updateColumns != null)
                    {
                        var updateSql = string.Join(",", target.updateColumns.Select(columnData => $"{columnData.Key} = '{columnData.Value}' "));
                        _db.ExecuteNonQueryAsync($"UPDATE {target.table} SET {updateSql} WHERE {target.where};").Wait();
                    }
                    else
                    {
                        _db.ExecuteNonQueryAsync($"DELETE FROM {target.table} WHERE {target.where};").Wait();
                    }
                }
            }
            finally
            {
                _db.Dispose();
            }
        }
    }
}
