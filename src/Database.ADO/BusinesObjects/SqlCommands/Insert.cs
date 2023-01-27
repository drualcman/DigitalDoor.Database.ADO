namespace Database.ADO.BusinesObjects.SqlCommands;

internal class Insert 
{
    readonly Commands Commands;
    public Insert(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        => Commands = new(dbLog, logResults, databaseControl, charControl, connectionString);

    #region direct queries
    public bool InsertInDB(string table, string colName, object colValue) => InsertInDB(table, new string[] { colName }, new object[] { colValue });

    public bool InsertInDB(string table, string[] colName, object[] colValue) => InsertInDB(table, colName, colValue, false) > 0;

    public int InsertInDB(string table, string[] colName, object[] colValue, bool returnScope)
    {
        int result = 0;
        if (!string.IsNullOrEmpty(table) && colName.Count() > 0 && colName.Count() == colValue.Count())
        {
            using SqlCommand cmd = SetInsert(table, colName, colValue);
            if (returnScope)
            {
                cmd.CommandText += "; select SCOPE_IDENTITY()";
                try
                {
                    result = Convert.ToInt32(Commands.Execute(cmd));
                }
                catch
                {
                    result = 0;
                }
            }
            else
            {
                result = Commands.ExecuteCommand(cmd) ? 1 : 0;
            }
        }
        return result;
    }

    #region tasks
    public async Task<int> InsertInDBAsync(string table, string[] colName, object[] colValue, bool returnScope)
    {
        int result = 0;
        if (!string.IsNullOrEmpty(table) && colName.Count() > 0 && colName.Count() == colValue.Count())
        {
            using SqlCommand cmd = SetInsert(table, colName, colValue);
            if (returnScope)
            {
                cmd.CommandText += " select SCOPE_IDENTITY()";
                try
                {
                    result = Convert.ToInt32(await Commands.ExecuteAsync(cmd));
                }
                catch
                {
                    result = 0;
                }
            }
            else
            {
                result = await Commands.ExecuteCommandAsync(cmd) ? 1 : 0;
            }
        }
        return result;
    }
    #endregion

    #endregion

    #region helpers
    private SqlCommand SetInsert(string table, string[] colName, object[] colValue)
    {
        StringBuilder columns = new StringBuilder();
        StringBuilder values = new StringBuilder();
        int i;

        SqlCommand cmd = new SqlCommand();
        //check columns
        for (i = 0; i < colName.Count(); i++)
        {
            columns.Append($"[{colName[i].Replace("[", "").Replace("]", "")}],");
            values.Append($"@value_{i},");

            if (colValue[i] is not null)
            {
                if (colValue[i].GetType() == typeof(DateTime))
                {
                    cmd.Parameters.AddWithValue("@value_" + i.ToString(), Convert.ToDateTime(colValue[i]).ToUniversalTime());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@value_" + i.ToString(), colValue[i]);
                }
            }
            else
            {
                cmd.Parameters.AddWithValue("@value_" + i.ToString(), DBNull.Value);
            }


        }
        columns.Remove(columns.Length - 1, 1);
        values.Remove(values.Length - 1, 1);
        cmd.CommandText = $"INSERT INTO {table} ({columns}) VALUES ({values});";
        return cmd;
    }
    #endregion
}
