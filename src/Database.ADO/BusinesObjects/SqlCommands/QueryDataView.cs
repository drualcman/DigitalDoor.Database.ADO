namespace Database.ADO.BusinesObjects.SqlCommands;

internal class QueryDataView : SqlQueryBase
{
    readonly QueryDataTable DataTables;

    public QueryDataView(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString)
        => DataTables = new(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString);

    #region direct queries
    public DataView GetDataView<TModel>(int timeout = 30) =>
        GetDataView(SetQuery<TModel>(), timeout);
    public DataView GetDataView(string sql, int timeout = 30)
    {
        log.start("GetDataView", sql, "");
        return DataTables.GetDataTable(sql, timeout).DefaultView;
    }
    #endregion

    #region tasks   
    public async Task<DataView> DataViewAsync<TModel>(int timeout = 30)
    {
        DataTable dt = await DataTables.GetDataTableAsync<TModel>(timeout);
        DataView dv = dt.DefaultView;
        return dv;
    }

    public async Task<DataView> DataViewAsync(string query, int timeout = 30)
    {
        log.start("DataViewAsync", query, "");
        DataTable dt = await DataTables.GetDataTableAsync(query, timeout);
        DataView dv = dt.DefaultView;
        return dv;
    }
    #endregion
}
