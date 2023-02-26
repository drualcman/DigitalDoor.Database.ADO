﻿namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class Update 
{
    readonly Commands Commands;
    public Update(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        => Commands = new(dbLog, logResults, databaseControl, charControl, connectionString);

    #region direct queries
    public bool UpdateColumn(string table, string colName, object colValue, string indexColumn, int index) => UpdateColumn(table, new string[] { colName }, new object[] { colValue }, new string[] { indexColumn }, new object[] { index });

    public bool UpdateColumn(string table, string colName, object colValue, string indexColumn, object index) => UpdateColumn(table, new string[] { colName }, new object[] { colValue }, new string[] { indexColumn }, new object[] { index });

    public bool UpdateColumn(string table, string[] colName, object[] colValue, string indexColumn, int index) => UpdateColumn(table, colName, colValue, new string[] { indexColumn }, new object[] { index });

    public bool UpdateColumn(string table, string[] colName, object[] colValue, string indexColumn, object index) => UpdateColumn(table, colName, colValue, new string[] { indexColumn }, new object[] { index });

    public bool UpdateColumn(string table, string[] colName, object[] colValue, string[] indexColumn, object[] index)
    {
        bool result;
        if (!string.IsNullOrEmpty(table) && colName.Count() > 0 && colName.Count() == colValue.Count() && indexColumn.Count() > 0)
        {
            using SqlCommand cmd = SetUpdate(table, colName, colValue, indexColumn, index);
            result = Commands.ExecuteCommand(cmd);

        }
        else
        {
            result = false;
        }
        return result;
    }
    #endregion

    #region tasks
    public async Task<bool> UpdateColumnAsync(string table, string colName, object colValue, string indexColumn, int index) =>
        await UpdateColumnAsync(table, new string[] { colName }, new object[] { colValue }, new string[] { indexColumn }, new object[] { index });
    public async Task<bool> UpdateColumnAsync(string table, string colName, object colValue, string indexColumn, object index) =>
        await UpdateColumnAsync(table, new string[] { colName }, new object[] { colValue }, new string[] { indexColumn }, new object[] { index });
    public async Task<bool> UpdateColumnAsync(string table, string[] colName, object[] colValue, string indexColumn, int index) =>
        await UpdateColumnAsync(table, colName, colValue, new string[] { indexColumn }, new object[] { index });
    public async Task<bool> UpdateColumnAsync(string table, string[] colName, object[] colValue, string indexColumn, object index) =>
        await UpdateColumnAsync(table, colName, colValue, new string[] { indexColumn }, new object[] { index });
    public async Task<bool> UpdateColumnAsync(string table, string[] colName, object[] colValue, string[] indexColumn, object[] index)
    {
        bool result;
        if (!string.IsNullOrEmpty(table) && colName.Count() > 0 && colName.Count() == colValue.Count() && indexColumn.Count() > 0)
        {
            using SqlCommand cmd = SetUpdate(table, colName, colValue, indexColumn, index);
            result = await Commands.ExecuteCommandAsync(cmd);

        }
        else
        {
            result = false;
        }
        return result;
    }
    #endregion

    #region helpers
    private SqlCommand SetUpdate(string table, string[] colName, object[] colValue, string[] indexColumn, object[] index)
    {
        int i;
        using SqlCommand cmd = new SqlCommand();
        StringBuilder sql = new StringBuilder($"UPDATE {table} SET ");
        //check columns
        for (i = 0; i < colName.Count(); i++)
        {
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
            sql.Append($" [{colName[i].Replace("[", "").Replace("]", "")}] = @value_{i},");
        }
        sql.Remove(sql.Length - 1, 1);
        if (indexColumn.Count() > 0)
        {
            sql.Append("  WHERE ");
            for (i = 0; i < indexColumn.Count(); i++)
            {
                cmd.Parameters.AddWithValue($"@index_{i}", index[i]);
                sql.Append($" [{indexColumn[i].Replace("[", "").Replace("]", "")}]  = @index_{i} AND ");
            }
            sql.Remove(sql.Length - 4, 4);
        }
        sql.Append(";");
        cmd.CommandText = sql.ToString();
        return cmd;
    }
    #endregion
}
