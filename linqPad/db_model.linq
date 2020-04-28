

void Main()
{
	// 產生類別名稱
	var nameOfTableAndClass = "ChatSettingModel";
	
	// 這邊修改為您要執行的 SQL Command
	var sqlCommand = $@" SELECT TotalStake,
                           TotalMemberReturn,
                           CommisionStake
                    FROM dbo.Wager";
	
	this.Connection.DumpClass(sqlCommand.ToString(), nameOfTableAndClass).Dump();
}

public static class LINQPadExtensions
{
    private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string> {
        { typeof(int), "int" },
        { typeof(short), "short" },
        { typeof(byte), "byte" },
        { typeof(byte[]), "byte[]" },
        { typeof(long), "long" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(float), "float" },
        { typeof(bool), "bool" },
        { typeof(string), "string" }
    };

    private static readonly HashSet<Type> NullableTypes = new HashSet<Type> {
        typeof(int),
        typeof(short),
        typeof(long),
        typeof(double),
        typeof(decimal),
        typeof(float),
        typeof(bool),
        typeof(DateTime),
		typeof(byte)
    };

    private static readonly Dictionary<Type, object> DefaultValue = new Dictionary<Type, object>() {
        { typeof(int),0},
        { typeof(double),0},
        { typeof(short),0},
        { typeof(long),0},
        { typeof(float),0},
        { typeof(decimal),0},
	    { typeof(bool),true},
		{typeof(byte),0},
        { typeof(string),string.Empty},
        { typeof(DateTime),"(DateTime?)null"},
	    { typeof(Byte[]),"{0}"},
    };

    private static readonly Dictionary<Type, string> ConvertString = new Dictionary<Type, string>() {
        { typeof(int),"Convert.ToInt32"},
		{typeof(byte),"Convert.ToByte"},
        { typeof(double),"Convert.ToDouble"},
        { typeof(short),"Convert.ToInt16"},
        { typeof(long),"Convert.ToInt64"},
        { typeof(float),"Convert.ToSingle"},
        { typeof(decimal),"Convert.ToDecimal"},
		{ typeof(bool),"Convert.ToBoolean"},
        { typeof(string),"Convert.ToString"},
        { typeof(DateTime),"Convert.ToDateTime"},
		{ typeof(Byte[]),"Convert.ToString"}
    };

    public static string DumpClass(this IDbConnection connection, string sql,
        string className = "Info",
        CommandType commandType = CommandType.Text)
    {

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        var cmd = connection.CreateCommand();
        cmd.CommandType = commandType;
        cmd.CommandText = sql;
        var reader = cmd.ExecuteReader();
        var builder = new StringBuilder();
		
        do
        {
		    DataTable schema = reader.GetSchemaTable();
            builder.Append(CreateModel(className, schema));
            builder.AppendLine();
            builder.Append(CreateMapper(className, schema));
        } while (reader.NextResult());

        return builder.ToString();
    }

    private static string CreateModel(string className, DataTable schema)
    {
        var builder = new StringBuilder();
        builder.AppendFormat("public class {0}{1}", className, Environment.NewLine);
        builder.AppendLine("{");

        foreach (DataRow row in schema.Rows)
        {
            var type = (Type)row["DataType"];
            var name = TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.Name;
            var isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);
            var columnName = (string)row["ColumnName"];

            builder.AppendLine($"\tpublic {name}{(isNullable ? "?" : string.Empty)} {columnName} {{ get; set; }}");
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string CreateMapper(string className, DataTable schema)
    {
        var builder = new StringBuilder();
        builder.AppendFormat("internal {0} {0}Liability(IDataReader dr){1}", className, Environment.NewLine);
        builder.AppendLine("{");
        builder.AppendLine($"\t{className} model = new {className}();");
        foreach (DataRow row in schema.Rows)
        {
            var type = (Type)row["DataType"];
            var collumnName = (string)row["ColumnName"];
            var isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);

            builder.AppendFormat("\tmodel.{0} = {1};",
                collumnName,
                GetConvertCode(type, collumnName, isNullable));

            builder.AppendLine();
        }
        builder.AppendLine("\treturn model;");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string GetConvertCode(Type type, string columnName, bool isNullable)
    {
        string drName = $"dr[\"{columnName}\"]";

        string normalConvert = $"{ConvertString[type]}({drName.ToLower()})";

        return isNullable ? 
            $"{drName} == DBNull.Value ? {DefaultValue[type].ToString().ToLower()} : " +
            $"{normalConvert}" : normalConvert;
    }
}