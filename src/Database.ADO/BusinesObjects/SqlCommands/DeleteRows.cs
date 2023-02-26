namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class DeleteRows
{
    readonly Commands Commands;

    public DeleteRows(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        => Commands = new(dbLog, logResults, databaseControl, charControl, connectionString);

    #region direct queries
    public bool DeleteRow(string table, string indexColumn, int index) => DeleteRow(table, new string[] { indexColumn }, new int[] { index });

    public bool DeleteRow(string table, string indexColumn, string index) => DeleteRow(table, new string[] { indexColumn }, new object[] { index });

    public bool DeleteRow(string table, string[] indexColumn, object[] index)
    {
        bool result;
        if (!string.IsNullOrEmpty(table) && indexColumn.Count() > 0 && index.Count() > 0)
        {
            string sql = $@"Delete FROM {table} WHERE ";  
            int i;
            using SqlCommand cmd = new SqlCommand();
            for (i = 0; i < indexColumn.Count(); i++)
            {
                sql += $"{indexColumn[i]} = @{indexColumn[i]}";
                cmd.Parameters.AddWithValue($"@{indexColumn[i]}", index[i]);
                if (i + 1 < indexColumn.Count()) sql += " AND ";
            }
            cmd.CommandText = sql;
            result = Commands.ExecuteCommand(cmd);
        }
        else result = false;
        return result;
    }

    public bool DeleteRow(string table, string[] indexColumn, int[] index)
    {
        bool result;
        if (!string.IsNullOrEmpty(table) && indexColumn.Count() > 0 && index.Count() > 0)
        {
            string sql = $@"Delete FROM {table} WHERE ";
            int i;
            using SqlCommand cmd = new SqlCommand();
            for (i = 0; i < indexColumn.Count(); i++)
            {
                sql += $"{indexColumn[i]} = @{indexColumn[i]}";
                cmd.Parameters.AddWithValue($"@{indexColumn[i]}", index[i]);
                if (i + 1 < indexColumn.Count()) sql += " AND ";
            }
            cmd.CommandText = sql;
            result = Commands.ExecuteCommand(cmd);

        }
        else result = false;
        return result;
    }
    #endregion

    #region Tasks
    public async Task<bool> DeleteRowAsync(string table, string indexColumn, int index) =>
        await DeleteRowAsync(table, new string[] { indexColumn }, new int[] { index });
    public async Task<bool> DeleteRowAsync(string table, string indexColumn, string index) =>
        await DeleteRowAsync(table, new string[] { indexColumn }, new object[] { index });

    public async Task<bool> DeleteRowAsync(string table, string[] indexColumn, object[] index)
    {
        bool result;
        if (!string.IsNullOrEmpty(table) && indexColumn.Count() > 0 && index.Count() > 0)
        {
            string sql = $@"Delete FROM {table} WHERE ";
            int i;
            using SqlCommand cmd = new SqlCommand();
            for (i = 0; i < indexColumn.Count(); i++)
            {
                sql += $"{indexColumn[i]} = @{indexColumn[i]}";
                cmd.Parameters.AddWithValue($"@{indexColumn[i]}", index[i]);
                if (i + 1 < indexColumn.Count()) sql += " AND ";
            }
            cmd.CommandText = sql;
            result = await Commands.ExecuteCommandAsync(cmd);

        }
        else result = false;
        return result;
    }
    public async Task<bool> DeleteRowAsync(string table, string[] indexColumn, int[] index)
    {
        bool result;
        if (!string.IsNullOrEmpty(table) && indexColumn.Count() > 0 && index.Count() > 0)
        {
            string sql = $@"Delete FROM {table} WHERE ";
            int i;
            using SqlCommand cmd = new SqlCommand();
            for (i = 0; i < indexColumn.Count(); i++)
            {
                sql += $"{indexColumn[i]} = @{indexColumn[i]}";
                cmd.Parameters.AddWithValue($"@{indexColumn[i]}", index[i]);
                if (i + 1 < indexColumn.Count()) sql += " AND ";
            }
            cmd.CommandText = sql;
            result = await Commands.ExecuteCommandAsync(cmd);

        }
        else result = false;
        return result;
    }
    #endregion
}
