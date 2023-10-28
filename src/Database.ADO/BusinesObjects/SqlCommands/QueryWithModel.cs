namespace Database.ADO.BusinesObjects.SqlCommands;

internal sealed class QueryWithModel
{
    private readonly QueryListWithModel QueryListWithModel;

    public QueryWithModel(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        => QueryListWithModel = new(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString);


    #region direct queries                           
    /// <summary>
    /// Executer query and return List of model send
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="query"></param>  
    /// <param name="timeout"></param>
    /// <returns></returns>
    public TModel Get<TModel>(string query = "", int timeout = 30) where TModel : new()
    {
        List<TModel> list = QueryListWithModel.ModelList<TModel>(query, timeout);
        if (list.Any()) return list[0];
        else return new TModel();
    }
    #endregion

    #region async 
    /// <summary>
    /// Executer query and return List of model send
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="query"></param> 
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<TModel> GetAsync<TModel>(string query = "", int timeout = 30) where TModel : new()
    {
        List<TModel> list = await QueryListWithModel.ModelListAsync<TModel>(query, timeout);
        if (list.Any()) return list[0];
        else return new TModel();
    }
    #endregion
}
