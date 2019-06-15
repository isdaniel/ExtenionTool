using System;
using Microsoft.SqlServer.Server;

namespace ExtensionTool
{
    internal sealed class IntMetaDataProvider : MetaDataProviderBase
    {
        internal override SqlMetaData GetSqlMetaData()
        {
            return new SqlMetaData(Name, DbType);
        }
        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetInt32(Order, (int)Value);
            }
        }
    }
}