using System;
using System.Collections.Generic;
using System.Text;
using LINQtoCSV;

namespace DataChecker2.Dao.Datas
{
    class DataCheck 
    {
        [CsvColumn(Name = "category")]
        public string Category;

        [CsvColumn(Name = "filename")]
        public string FileName;

        [CsvColumn(Name = "property")]
        public string Property;

        [CsvColumn(Name = "aggrfilename")]
        public string AggrFileName;


        [CsvColumn(Name = "aggrproperty")]
        public string AggrProperty;


        public bool IsKey;

        [CsvColumn(Name = "iskey")]
        public string _IsKey
        {
            set => IsKey = value?.ToString().ToLower() == "true";
        }


        public bool Nullable;
        [CsvColumn(Name = "nullable")]
        public string _Nullable
        {
            set => Nullable = value?.ToString().ToLower() == "true";
        }


        public bool Zeroable;
        [CsvColumn(Name = "zeroable")]
        public string _Zeroable
        {
            set => Zeroable = value?.ToString().ToLower() == "true";
        }

        public bool CustomAction;
        [CsvColumn(Name = "customaction")]
        public string _CustomAction
        {
            set => CustomAction = value?.ToString().ToLower() == "true";
        }




        protected long Parse(object value)
        {
            if (value != null && long.TryParse(value.ToString(), out var result))
            {
                return result;
            }

            return 0;
        }
    }
}
