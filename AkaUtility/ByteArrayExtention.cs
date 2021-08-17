using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Globalization;

namespace AkaUtility
{
    public static class ByteArrayExtention
    {
        public static IList<T> ConvertCsvToList<T>(this byte[] csvBytes)
        {
            using (var memStream = new MemoryStream(csvBytes, 0, csvBytes.Length, true, true))
            {
                memStream.Position = 0;

                using (StreamReader reader = new StreamReader(memStream, Encoding.UTF8))
                {
                    using (var csv = new CsvReader(reader, new CultureInfo("en-US")))
                    {
                        csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Trim();
                        csv.Configuration.Delimiter = "|";

                        IList<T> rows = new List<T>();
                        int count = 1;
                        while (csv.Read())
                        {
                            count++;
                            try
                            {
                                var data = csv.GetRecord<T>();
                                rows.Add(data);
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Error rows : " + count);
                            }

                        }

                        return rows;
                    }
                }
            }
        }

        public static int aa = 0;
        public static IList<T> ConvertCsvToListWithMapping<T, TMap>(this byte[] csvBytes)
            where TMap : ClassMap
        {
            using (var memStream = new MemoryStream(csvBytes, 0, csvBytes.Length, true, true))
            {
                memStream.Position = 0;

                using (StreamReader reader = new StreamReader(memStream, Encoding.UTF8))
                {
                    using (var csv = new CsvReader(reader, new CultureInfo("en-US")))
                    {
                        csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Trim();
                        csv.Configuration.RegisterClassMap<TMap>();
                        csv.Configuration.Delimiter = "|";

                        IList<T> rows = new List<T>();

                        int count = 1;
                        while (csv.Read())
                        {
                            count++;
                            try
                            {
                                var data = csv.GetRecord<T>();
                                rows.Add(data);
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Error rows : " + count);
                            }

                        }
                        return rows;
                    }
                }
            }
        }
    }
}
