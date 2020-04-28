
void Main()
{
    string referTable = "Product";
    string foreignKey = "ProductID";
	// 這邊修改為您要執行的 SQL Command
	this.Connection.DumpClass(foreignKey,referTable);
	
	"OK".Dump();
}

public static class LINQPadExtensions
{
	private static string CheckDirectoryPath(IDbConnection connection,string forlder1,string forlder2){
		string path = $@"D:\database\Constraints\{forlder1}\{forlder2}\";
		
		if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
				
		return path;
	}


    public static void DumpClass(this IDbConnection connection, string foreignKey,string referTable)
    {
		string[] dbPath = connection.Database.Split('_');
        string SavePath = CheckDirectoryPath(connection,dbPath[0],dbPath[1]);
		
		string GetFkTableSql = $@"SELECT t.TABLE_NAME
		FROM INFORMATION_SCHEMA.TABLES t 
		JOIN  INFORMATION_SCHEMA.COLUMNS c  on c.TABLE_NAME = t.TABLE_NAME
		LEFT JOIN (
		    SELECT 
		        OBJECT_NAME(f.parent_object_id) TableName,
		        COL_NAME(fc.parent_object_id,fc.parent_column_id) fk_Name
		    FROM 
		        sys.foreign_keys AS f JOIN 
		        sys.foreign_key_columns AS fc 
		            ON f.OBJECT_ID = fc.constraint_object_id
		) t1 on  t1.TableName = t.TABLE_NAME and t1.fk_Name = c.COLUMN_NAME
		WHERE t.TABLE_TYPE = 'BASE TABLE' 
		AND t.TABLE_CATALOG='{connection.Database}' 
		AND c.COLUMN_NAME = '{foreignKey}' 
		AND t1.TableName IS NULL";
		
        if (connection.State != ConnectionState.Open)
            connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = GetFkTableSql;
		
		using (var dr = cmd.ExecuteReader())
        {  
            while (dr.Read())
            {
               StringBuilder sb = new StringBuilder();
                string FKName = $"FK_{dr["TABLE_NAME"]}_{referTable}";
                sb.AppendLine($"USE [{dbPath[0]}_{dbPath[1]}]");
                sb.AppendLine("GO");
                sb.AppendLine($@"IF EXISTS (SELECT * 
                FROM sys.foreign_keys
                WHERE object_id = OBJECT_ID(N'[dbo].[{FKName}]')
                AND parent_object_id = OBJECT_ID(N'[dbo].[{dr["TABLE_NAME"]}]'))");
                sb.AppendLine("BEGIN");
                sb.AppendLine($@"ALTER TABLE [dbo].[{dr["TABLE_NAME"]}]
    DROP CONSTRAINT {FKName}");
                sb.AppendLine("END");
				sb.AppendLine($"ALTER TABLE [dbo].[{dr["TABLE_NAME"]}]");
				sb.AppendLine($"WITH CHECK ADD  CONSTRAINT {FKName}");
				sb.AppendLine($"FOREIGN KEY({foreignKey})");
				sb.AppendLine($"REFERENCES [dbo].[{referTable}]({foreignKey})");
                File.WriteAllText($"{SavePath}{FKName}.Constraint.sql", sb.ToString());
            }
        }
    }
}