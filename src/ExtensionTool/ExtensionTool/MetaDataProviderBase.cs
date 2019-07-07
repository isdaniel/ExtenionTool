using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace ExtensionTool
{
    internal abstract class MetaDataProviderBase
    {
        internal string Name { get; set; }
        internal byte Scale { get; set; }
        internal byte Precision { get; set; }
        internal long Length { get; set; }
        internal SqlDbType DbType { get; set; }
        internal int Order { get; set; }

        internal object Value { get; set; }

        internal virtual SqlMetaData GetSqlMetaData()
        {
            return new SqlMetaData(Name, DbType);
        }

        public abstract Action<SqlDataRecord> SetRecordValue { get; }
    }
}