using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQtoCSV;

namespace DataChecker2.Dao.Datas
{
    class CommonDataRow : List<DataRowItem> , IDataRow
    {
    }

    class DataRow : List<string>
    {
        private readonly Dictionary<string, int> _indexer = new Dictionary<string, int>();

        public string this[string key] => this[_indexer[key]];

        public void AddKey(string key, int index) => _indexer.Add(key, index);

        public DataRow(CommonDataRow rawData)
        {
            this.AddRange(rawData.Select(data => data.Value?.Trim().ToLower()));
        }
    }

}
