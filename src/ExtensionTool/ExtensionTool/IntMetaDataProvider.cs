using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace ExtensionTool
{
    internal sealed class IntMetaDataProvider : MetaDataProviderBase
    {
        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetInt32(Order, (int)Value);
            }
        }
    }

    internal sealed class LongMetaDataProvider : MetaDataProviderBase
    {
        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetInt64(Order, (long)Value);
            }
        }
    }

    internal sealed class ByteMetaDataProvider : MetaDataProviderBase
    {
        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetByte(Order, (byte)Value);
            }
        }
    }

    internal sealed class ShortMetaDataProvider : MetaDataProviderBase
    {
        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetInt16(Order, (short)Value);
            }
        }
    }

    internal sealed class DateTimeMetaDataProvider : MetaDataProviderBase
    {
        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetDateTime(Order, (DateTime)Value);
            }
        }
    }

    internal sealed class DecimalMetaDataProvider : MetaDataProviderBase
    {
        internal override SqlMetaData GetSqlMetaData()
        {
            if (DbType == SqlDbType.Decimal)
            {
                return new SqlMetaData(Name, DbType, Precision, Scale);
            }

            return base.GetSqlMetaData();
        }

        public override Action<SqlDataRecord> SetRecordValue
        {
            get
            {
                return (record) => record.SetDecimal(Order, (decimal)Value);
            }
        }
    }
}