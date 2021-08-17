using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQtoCSV;

namespace DataChecker2.Dao.Datas
{
    class CommonData : IReadOnlyList<DataRow>
    {
        private readonly Dictionary<string, List<string>> _rawDatasForColumn = new Dictionary<string, List<string>>();
        private readonly List<DataRow> _rawDatas = new List<DataRow>();
        private readonly List<string> _columns = new List<string>();


        public DataRow this[int index] => _rawDatas[index];
        public List<string> this[string property] => TryGetValue(property, out var list) ? list : null;

        public int Count => _rawDatas.Count;

        public IEnumerator GetEnumerator()
        {
            return _rawDatas.GetEnumerator();
        }

        IEnumerator<DataRow> IEnumerable<DataRow>.GetEnumerator()
        {
            return _rawDatas.GetEnumerator();
        }

        public bool TryGetValue(string property , out List<string> result)
        {
            result = null;
            return _rawDatasForColumn.TryGetValue(property, out result);
        }


        public static CommonData LoadDataFromFile(string file)
        {
            var loader = new CsvFileDescription
            {
                SeparatorChar = '|',
                FirstLineHasColumnNames = false,
                TextEncoding = Encoding.UTF8
            };
            var csvContext = new CsvContext();
            var rows = csvContext.Read<CommonDataRow>(file, loader);
            var commonData = new CommonData();;
            if (rows.Any() == false)
                return commonData;

            using (var enumerator = rows.GetEnumerator())
            {
                enumerator.MoveNext();
                var row = enumerator.Current;
                foreach (var column in row)
                {
                    commonData._columns.Add(column.Value.Trim());
                    commonData._rawDatasForColumn.Add(column.Value.Trim(), new List<string>());
                }

                while (enumerator.MoveNext())
                {
                    var data = new DataRow(enumerator.Current);
                    for (int i = 0; i < row.Count; i++)
                    {
                        data.AddKey(commonData._columns[i], i);
                        commonData._rawDatasForColumn[commonData._columns[i]].Add(row[i].Value?.Trim().ToLower());
                    }
                    commonData._rawDatas.Add(data);
                }
            }
            return commonData;
        }
    }

}
