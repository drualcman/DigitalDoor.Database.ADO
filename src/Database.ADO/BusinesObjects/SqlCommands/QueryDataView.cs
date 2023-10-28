namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class QueryDataView : SqlQueryBase
{
    readonly QueryDataTable DataTables;

    public QueryDataView(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString)
        => DataTables = new(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString);

    #region direct queries
    public DataView GetDataView<TModel>(int timeout = 30, string indexColumn = "", int pageNumber = 0, int numElements = 0) =>
        GetDataView(SetQuery<TModel>(indexColumn, pageNumber, numElements), timeout);
    public DataView GetDataView(string sql, int timeout = 30)
    {
        Log.start("GetDataView", sql, "");
        return DataTables.GetDataTable(sql, timeout).DefaultView;
    }
    #endregion

    #region tasks   
    public async Task<DataView> DataViewAsync<TModel>(int timeout = 30, string indexColumn = "", int pageNumber = 0, int numElements = 0)
    {
        DataTable dt = await DataTables.GetDataTableAsync<TModel>(timeout, indexColumn, pageNumber, numElements);
        DataView dv = dt.DefaultView;
        return dv;
    }

    public async Task<DataView> DataViewAsync(string query, int timeout = 30)
    {
        Log.start("DataViewAsync", query, "");
        DataTable dt = await DataTables.GetDataTableAsync(query, timeout);
        DataView dv = dt.DefaultView;
        return dv;
    }
    #endregion
}
