
void Main()
{
	// Sp名稱
	var SpName = "[dbo].[TxnBetInfo_GetInfoByBetNo]";
	bool IsTransaction = false;
	this.Connection.DumpSpParameter(SpName,IsTransaction).Dump();
}

    public static class LINQPadExtensions
    {
        private static string SpSql = @"select  
	        'Parameter_name' = name,  
	        'Type' = type_name(user_type_id),  
	        'Length' = max_length,  
	        'Prec' = case when type_name(system_type_id) = 'uniqueidentifier'
	        then precision  
	        else OdbcPrec(system_type_id, max_length, precision) end,  
	        'Scale' = OdbcScale(system_type_id, scale),  
	        'Param_order' = parameter_id
	        from sys.parameters where object_id = object_id(@Name)
	        ORDER BY parameter_id";

        private static string uftt_Sql = @"SELECT   
				        [ColumnName],
				        [DataType],
				        max_length,
				        precision,
				        scale,
				        is_nullable
				FROM (
				    Select
					    c.name   [ColumnName]
					    ,y.name   [DataType]
					    ,SUM(CASE WHEN y.name  = 'sysname' THEN - c.max_length / 2 ELSE c.max_length END)
				            OVER(PARTITION BY c.name,c.precision,c.scale,c.is_nullable )
				         max_length
					    ,c.precision
					    ,c.scale
					    ,c.is_nullable
				        ,c.column_id
				    From sys.table_types t
				    Inner join sys.columns c on c.object_id = t.type_table_object_id
				    Inner join sys.types y ON y.system_type_id = c.system_type_id
				    WHERE t.is_user_defined = 1
				    AND t.is_table_type = 1
				    AND t.name =  @Name
				) t1
				WHERE [DataType] <> 'sysname'
				ORDER BY column_id";

        private static Dictionary<string, DbType> typeMapper = new Dictionary<string, DbType>()
        {
            {"smallint", DbType.Int16},
            {"int", DbType.Int32},
            {"datetime2", DbType.DateTime2},
            {"datetime", DbType.DateTime},
            {"varchar", DbType.AnsiString},
            {"date", DbType.Date},
            {"bigint", DbType.Int64},
            {"tinyint", DbType.Byte},
            {"numeric",DbType.Decimal},
            { "nchar",DbType.StringFixedLength},
			{ "char",DbType.AnsiStringFixedLength},
            { "nvarchar",DbType.String},
            { "sysname",DbType.String},
            { "money",DbType.Decimal},
			{ "decimal",DbType.Decimal},
			{ "bit",DbType.Boolean}
        };

        private static Dictionary<DbType,string> utffSetMapper= new Dictionary<DbType,string>()
        {
            {DbType.Int16,"SetInt16"},
            {DbType.Int32,"SetInt32"},
            {DbType.DateTime2,"SetDateTime"},
            {DbType.DateTime,"SetDateTime"},
            {DbType.String,"SetStringOrNull"},
			{DbType.AnsiString,"SetStringOrNull"},
			{DbType.StringFixedLength,"SetStringOrNull"},
			{DbType.AnsiStringFixedLength,"SetStringOrNull"},
            {DbType.Date,"SetDateTime"},
            {DbType.Int64,"SetInt64"},
            {DbType.Byte,"SetByte"},
            {DbType.Decimal,"SetDecimal"},
			{DbType.Boolean,"SetBoolean"}
        };

        private static Dictionary<string, string> metaDataMapper = new Dictionary<string, string>()
        {
            {"smallint", "SqlDbType.SmallInt"},
            {"int", "SqlDbType.Int"},
            {"datetime2", "SqlDbType.DateTime2"},
            {"datetime", "SqlDbType.DateTime"},
            {"varchar", "SqlDbType.VarChar"},
            {"nvarchar", "SqlDbType.NVarChar"},
            {"date", "SqlDbType.Date"},
            {"bigint", "SqlDbType.BigInt"},
            {"tinyint", "SqlDbType.TinyInt"},
            {"numeric","SqlDbType.Decimal"},
            {"char","SqlDbType.Char"},
			{"nchar","SqlDbType.NChar"},
            {"money","SqlDbType.Money"},
			{"decimal","SqlDbType.Decimal"},
			{ "bit","SqlDbType.Bit"}
        };
              
        public static string DumpSpParameter(this IDbConnection connection, string spName,bool IsTransaction)
        {
            StringBuilder sb = new StringBuilder();

            var parameterInfo = connection.GetSpInfo(spName);

            foreach (var para in parameterInfo)
            {
                sb.AppendLine(GetParameterString(para));
            }
			$"var dapperContext = GetDapperContext(\"{spName}\", {IsTransaction.ToString().ToLower()});".Dump();
            var utff_List = parameterInfo.Where(x => x.Length < 0);
            foreach (var utff in utff_List)
            {
                StringBuilder utffBuilder= new StringBuilder();
                var UtffInfo = connection.GetUtffInfo(utff.Type);
                utffBuilder.AppendLine($"public IEnumerable<SqlDataRecord> ConvertTo{utff.Name.FirstCharToUpper()}DataRecord(IEnumerable<{utff.Name}> parameters){{");

                utffBuilder.AppendLine("var sqlMetaData = new SqlMetaData[]{");

                utffBuilder.AppendLine(string.Join("\r\n", UtffInfo.Select(GetSqlMetaDataString)));
				
                utffBuilder.AppendLine("};");

                utffBuilder.AppendLine("var mappingDictionarys = sqlMetaData.ToMappingDictionary();");
                utffBuilder.AppendLine("var records = new List<SqlDataRecord>();");
			    utffBuilder.AppendLine("if(parameters!=null)");
				utffBuilder.AppendLine("{");
                utffBuilder.AppendLine("foreach (var para in parameters){");
                utffBuilder.AppendLine("var record = new SqlDataRecord(sqlMetaData);");

                utffBuilder.AppendLine(string.Join("\r\n", UtffInfo.Select(GetParameterString))); 

                utffBuilder.AppendLine("records.Add(record);");
                utffBuilder.AppendLine("}");
				utffBuilder.AppendLine("}");
                utffBuilder.AppendLine("return records;");
                utffBuilder.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine(utffBuilder.ToString());
            }

           

            return sb.ToString();
        }

		private static string FirstCharToUpper(this string input)
		{
		    if (String.IsNullOrEmpty(input))
		        throw new ArgumentException("ARGH!");
		    return input.First().ToString().ToUpper() + input.Substring(1);
		}
		
        private static string GetSqlMetaDataString(UTTFParameterModel model)
        {
            string result =string.Empty;
  
            if (new[]{
                "smallint",
                "int",
                "bigint",
                "tinyint",
                "datetime2",
                "datetime",
                "date",
                "money",
				"bit" }.Contains(model.DataType))
            {
                result = $"\tnew SqlMetaData(\"{model.ColumnName}\", {metaDataMapper[model.DataType]}),";
            }
            else if(new[]{
                "varchar",
                "nvarchar",
                "char",
				"nchar"}.Contains(model.DataType))
            {
                result = $"\tnew SqlMetaData(\"{model.ColumnName}\", {metaDataMapper[model.DataType]},{model.max_length}),";
            }
            else if(new[]{
                "numeric",
				"decimal"}.Contains(model.DataType))
            {
                result = $"\tnew SqlMetaData(\"{model.ColumnName}\", {metaDataMapper[model.DataType]},{model.precision},{model.scale}),";
            }

            return result;
        }
       
        private static string GetParameterString(UTTFParameterModel para)
        {

            string result = $"\trecord.{utffSetMapper[typeMapper[para.DataType]]}(mappingDictionarys[\"{para.ColumnName}\"], para.{para.ColumnName});";

            return result;
        }

        private static string GetParameterString(StoreProcedureParameterModel para)
        {
            string result = string.Empty;
 		    string ParameterAdd = "dapperContext.Parameters.Add";
            if (para.Length < 0)
            {
                result = $"{ParameterAdd}(\"{para.Parameter_name}\",ConvertTo{para.Name.FirstCharToUpper()}DataRecord({para.Name}).AsTableValuedParameter(\"dbo.{para.Type}\"));";
            }
            else if (new[]
            {
                DbType.Int16,
                DbType.Int32,
                DbType.Int64,
                DbType.DateTime2,
                DbType.DateTime,
			    DbType.Boolean,
				DbType.Byte
            }.Contains(typeMapper[para.Type]))
            {
                result =
                    $"{ParameterAdd}(\"{para.Parameter_name}\",{para.Name},DbType.{typeMapper[para.Type]},ParameterDirection.Input);";
            }
            else if (new[] {DbType.String, DbType.AnsiString}.Contains(typeMapper[para.Type]))
            {
                result =
                    $"{ParameterAdd}(\"{para.Parameter_name}\",{para.Name},DbType.{typeMapper[para.Type]},ParameterDirection.Input,{para.Length});";
            }
			 else if (new[] {DbType.Decimal }.Contains(typeMapper[para.Type]))
            {
                result =
                    $"{ParameterAdd}(\"{para.Parameter_name}\",{para.Name},DbType.{typeMapper[para.Type]},ParameterDirection.Input,precision:{para.Prec.Value},scale:{para.Scale.Value});";
            }

            return result;
        }

        private static List<UTTFParameterModel> GetUtffInfo(this IDbConnection connection, string Name)
        {
            var cmd = CreateCommand(connection);
            cmd.CommandText = uftt_Sql;
            var para1 = new SqlParameter("@Name", SqlDbType.VarChar) { Value = Name };
            cmd.Parameters.Add(para1);
            List<UTTFParameterModel> result = new List<UTTFParameterModel>();

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    UTTFParameterModel model = new UTTFParameterModel
                    {
                        ColumnName = Convert.ToString(dr["ColumnName"]),
                        DataType = Convert.ToString(dr["DataType"]),
                        max_length = Convert.ToInt16(dr["max_length"]),
                        precision = Convert.ToByte(dr["precision"]),
                        scale = Convert.ToByte(dr["scale"]),
                        is_nullable = dr["is_nullable"] == DBNull.Value ? true : Convert.ToBoolean(dr["is_nullable"])
                    };
                    result.Add(model);
                }
            }
           
            return result;
        }

        private static List<StoreProcedureParameterModel> GetSpInfo(this IDbConnection connection, string SpName)
        {
            List<StoreProcedureParameterModel> result = new List<StoreProcedureParameterModel>();

            var cmd = CreateCommand(connection);
            cmd.CommandText = SpSql;

            var para1 = new SqlParameter("@Name", SqlDbType.VarChar) {Value = SpName};
            cmd.Parameters.Add(para1);
            using (var dr = cmd.ExecuteReader())
            {
                
            while (dr.Read())
            {
                StoreProcedureParameterModel model = new StoreProcedureParameterModel
                {
                    Parameter_name = dr["Parameter_name"]?.ToString(),
                    Type = dr["Type"]?.ToString(),
                    Length = Convert.ToInt16(dr["Length"]),
                    Param_order = Convert.ToInt32(dr["Param_order"]),
                    Prec = dr["Prec"] != DBNull.Value ? Convert.ToInt32(dr["Prec"]) : 0,
                    Scale = dr["Scale"] != DBNull.Value ? Convert.ToInt32(dr["Scale"]) : 0
                };
                result.Add(model);
            }

            }

            return result;
        }

        private static IDbCommand CreateCommand(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            return cmd;
        }
    }
    public class UTTFParameterModel
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public short max_length { get; set; }
        public byte precision { get; set; }
        public byte scale { get; set; }
        public bool? is_nullable { get; set; }
    }

    public class StoreProcedureParameterModel
    {
        public string Parameter_name { get; set; }
        public string Type { get; set; }
        public short Length { get; set; }
        public int? Prec { get; set; }
        public int? Scale { get; set; }
        public int Param_order { get; set; }

        public string Name{
            get { return Parameter_name.Replace("@", ""); }
        }
    }