using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace ExtensionTool
{
    public static class SqlDataRecordExtension
    {
        private static MetaDataProviderBase GetMetaDataProvider(Type t)
        {
            if (typeof(int) == t)
                return new IntMetaDataProvider();
            else if (typeof(string) == t)
                return new StringMetaDataProvider();
            else if (typeof(DateTime) == t)
                return new DateTimeMetaDataProvider();
            else if (typeof(long) == t)
                return new LongMetaDataProvider();
            else if (typeof(short) == t)
                return new ShortMetaDataProvider();
            else if (typeof(byte) == t)
                return new ByteMetaDataProvider();

            throw new Exception("Not support MetaDataProvider");
        }

        public static IEnumerable<SqlDataRecord> GetDataRecords<TModel>(this IEnumerable<TModel> list)
            where TModel : class, new()
        {
            var result = new List<SqlDataRecord>();

            if (list == null || !list.Any())
                return new List<SqlDataRecord>();
          
            var metaDataCollection = list.Select(GetMetaDataCollection);
            
            var metaData = metaDataCollection
                            .FirstOrDefault()
                            .Select(x => x.GetSqlMetaData()).ToArray();

            foreach (var providerBases in metaDataCollection)
            {
                var record = new SqlDataRecord(metaData);
                foreach (var item in providerBases)
                {
                    item.SetRecordValue(record);
                }
                result.Add(record);
            }

            return result;
        }

        private static IEnumerable<MetaDataProviderBase> GetMetaDataCollection<TModel>(TModel item) 
            where TModel : class, new()
        {
            var contents = typeof(TModel)
                .GetProperties()
                .Select((x, index) =>
                {
                    var attr = x.GetAttribute<MetaDataAttribute>(false);
                    var metaDataProvider = GetMetaDataProvider(x.PropertyType);
                    metaDataProvider.Name = string.IsNullOrEmpty(attr.Name) ? x.Name : attr.Name;
                    metaDataProvider.Precision = attr.Precision;
                    metaDataProvider.Scale = attr.Scale;
                    metaDataProvider.Length = attr.Length;
                    metaDataProvider.DbType = attr.DbType;
                    metaDataProvider.Order = index;
                    metaDataProvider.Value = x.GetValue(item);
                    return metaDataProvider;
                });
            return contents;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MetaDataAttribute : Attribute
    {
        public string Name { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public long Length { get; set; }
        public SqlDbType DbType { get; set; }
    }

}
