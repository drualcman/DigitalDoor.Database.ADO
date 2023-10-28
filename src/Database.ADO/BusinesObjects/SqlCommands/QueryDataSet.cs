namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class QueryDataSet : SqlQueryBase
{
    public QueryDataSet(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString) 
        : base(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString)
    {
    }

    #region direct queries    
    public DataSet GetDataSet<TModel>(int timeout = 30, string indexColumn = "", int pageNumber = 0, int numElements = 0) => GetDataSet(SetQuery<TModel>(indexColumn, pageNumber, numElements), timeout);
    public DataSet GetDataSet(string sql, int timeout = 30)
    {
        Log.start("GetDataSet", sql, "");
        QHelpers.CheckQuery(sql);
        try
        {
            sql = QueryHelpers.CleanSqlDataColumns(sql);

            using SqlConnection cn = new SqlConnection(ConnectionString);
            using SqlDataAdapter da = new SqlDataAdapter(sql, cn);
            da.SelectCommand.CommandTimeout = timeout;
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Log.end(null, ex.ToString());
                throw;
            }
            finally
            {
                if (LogResults) Log.end(ds);

            }

            return ds;
        }
        catch (Exception ex)
        {
            Log.end(sql, ex.ToString());

            throw;
        }

    }
    #endregion region

    #region tasks 
    public Task<DataSet> GetDataSetAsync<TModel>(int timeout = 30, string indexColumn = "", int pageNumber = 0, int numElements = 0) => GetDataSetAsync(SetQuery<TModel>(indexColumn, pageNumber, numElements), timeout);
    public Task<DataSet> GetDataSetAsync(string query, int timeout = 30) =>
        Task.FromResult(GetDataSet(query, timeout));
    #endregion

}
