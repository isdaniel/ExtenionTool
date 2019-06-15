using System;
using Microsoft.SqlServer.Server;

namespace ExtensionTool
{
    internal sealed class StringMetaDataProvider : MetaDataProviderBase
    {
        internal override SqlMetaData GetSqlMetaData()
        {
            return new SqlMetaData(Name, DbType, Length);
        }

        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetString(Order, (Value ?? string.Empty).ToString());
            }
        }
    }
}