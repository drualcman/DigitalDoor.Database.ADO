namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class Commands : SqlBaseCommands
{
    public Commands(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(dbLog, logResults, databaseControl, charControl, connectionString)
    {
    }

    #region direct query queries      
    public object Execute(string query, int timeout = 30)
    {
        Log.start("EjecutarSQL(query)", query, "");
        QHelpers.CheckQuery(query);
        using SqlCommand cmd = new SqlCommand();
        cmd.CommandText = query;
        object result;
        try
        {
            result = Execute(cmd, timeout);
            if(LogResults) Log.end(result);
        }
        catch(Exception ex)
        {
            result = null;
            Log.end(result, ex);
            throw;
        }
        finally
        {
            cmd.Dispose();
        }
        return result;
    }

    public bool ExecuteCommand(string query, int timeout = 30)
    {
        Execute(query, timeout);
        return true;
    }

    #region task
    public async Task<object> ExecuteAsync(string query, int timeout = 30)
    {
        object result;
        QHelpers.CheckQuery(query);
        using SqlCommand cmd = new SqlCommand();
        cmd.CommandText = query;
        result = await ExecuteAsync(cmd, timeout);
        return result;
    }

    public async Task<bool> ExecuteCommandAsync(string query, int timeout = 30)
    {
        bool result;
        QHelpers.CheckQuery(query);
        using SqlCommand cmd = new SqlCommand();
        cmd.CommandText = query;
        result = await ExecuteCommandAsync(cmd, timeout);
        return result;
    }
    #endregion
    #endregion

    #region methods    
    public bool ExecuteCommand(SqlCommand cmd, int timeout = 30)
    {
        Execute(cmd, timeout);
        return true;
    }
    public object Execute(SqlCommand cmd, int timeout = 30)
    {
        object result;
        Log.start("Execute(cmd)", cmd.CommandText, ConnectionString);
        try
        {
            using SqlConnection cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandTimeout = timeout;
            cmd.Connection.Open();
            cmd.CommandTimeout = timeout;
            if(cmd.CommandText.ToLower().StartsWith("create") || cmd.CommandText.ToLower().StartsWith("drop")) result = cmd.ExecuteNonQuery();
            else result = cmd.ExecuteScalar();
            cmd.Connection.Close();
            if(LogResults) Log.end(result?.ToString());
        }
        catch(Exception ex)
        {
            result = null;
            Log.end(result, ex);
            throw;
        }
        finally
        {
            cmd.Dispose();
        }
        return result;
    }
    #endregion

    #region task
    public async Task<bool> ExecuteCommandAsync(SqlCommand cmd, int timeout = 30)
    {
        await ExecuteAsync(cmd, timeout);
        return true;
    }

    public async Task<object> ExecuteAsync(SqlCommand cmd, int timeout = 30)
    {
        object result = null;
        if(cmd != null)
        {
            Log.start("Execute(cmd)", cmd.CommandText, ConnectionString);
            try
            {
                using SqlConnection cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandTimeout = timeout;
                await cmd.Connection.OpenAsync();
                cmd.CommandTimeout = timeout;
                if(cmd.CommandText.ToLower().StartsWith("create") || cmd.CommandText.ToLower().StartsWith("drop")) result = await cmd.ExecuteNonQueryAsync();
                else result = await cmd.ExecuteScalarAsync();
                await cmd.Connection.CloseAsync();
                if(LogResults) Log.end(result?.ToString());
            }
            catch(Exception ex)
            {
                result = null;
                Log.end(result, ex);
                throw;
            }
            finally
            {
                await cmd.DisposeAsync();
            }
        }
        else
        {
            Log.start("ExecuteCommand(cmd)", "", ConnectionString);
            Log.end(result.ToString(), "CMD is null");
        }
        return result;
    }
    #endregion
}
