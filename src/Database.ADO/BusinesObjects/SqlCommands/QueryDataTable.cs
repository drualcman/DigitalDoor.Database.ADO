namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class QueryDataTable : SqlQueryBase
{
    private readonly QueryDataSet DataSets;

    public QueryDataTable(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString)
        => DataSets = new(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString);

    #region direct queries    
    public DataTable GetDataTable<TModel>(int timeout = 30, string indexColumn = "", int pageNumber = 0, int numElements = 0) => GetDataTable(SetQuery<TModel>(indexColumn, pageNumber, numElements), timeout);
    public DataTable GetDataTable(string sql, int timeout = 30)
    {
        Log.start("GetDataTable", sql, "");
        return DataSets.GetDataSet(sql, timeout).Tables[0];
    }
    #endregion

    #region tasks  
    public Task<DataTable> GetDataTableAsync<TModel>(int timeout = 30, string indexColumn = "", int pageNumber = 0, int numElements = 0) => GetDataTableAsync(SetQuery<TModel>(indexColumn, pageNumber, numElements), timeout);
    public async Task<DataTable> GetDataTableAsync(string query, int timeout = 30)
    {
        Log.start("GetDataTableAsync", query, "");
        DataSet ds = await DataSets.GetDataSetAsync(query, timeout);
        DataTable dt = ds.Tables[0];
        ds.Dispose();
        return dt;
    }
    #endregion

}
