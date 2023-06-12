namespace Database.ADO.BusinesObjects.SqlCommands;
internal sealed class BulkCommands
{     
    readonly IDbLog Log;
    readonly bool LogResults;
    readonly string ConnectionString;

    public BulkCommands(IDbLog log, bool logResults, string connectionString)
    {
        Log = log;
        LogResults = logResults;
        ConnectionString = connectionString;
    }

    public async Task<bool> BulkCopyAsync(DataTable dt, string destinationTableName, int timeout)
    {
        bool result;
        try
        {
            using SqlConnection cn = new SqlConnection(ConnectionString);
            cn.Open();
            using SqlBulkCopy bulkCopy = new SqlBulkCopy(cn);
            bulkCopy.BulkCopyTimeout = timeout;
            bulkCopy.DestinationTableName = destinationTableName;
            await bulkCopy.WriteToServerAsync(dt);
            bulkCopy.Close();
            cn.Close();
            result = true;
        }
        catch(Exception ex)
        {
            result = false;
            Log.end(result, ex);
            throw;
        }
        if(LogResults) Log.end(result.ToString());
        return result;
    }
}
