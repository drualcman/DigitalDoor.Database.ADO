namespace Database.ADO.BusinesObjects.SqlCommands;

internal class QueryListWithModel : SqlQueryBase
{
    public QueryListWithModel(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(requiredFields, dbLog, logResults, databaseControl, charControl, connectionString)
    {
    }

    #region direct queries
    public List<TModel> ModelList<TModel>(string sql = "", int timeout = 30) where TModel : new() =>
        ModelListAsync<TModel>(sql, timeout).Result;

    public List<TModel> ModelList<TModel>(SqlCommand cmd, int timeout = 30) where TModel : new() =>
        ModelListAsync<TModel>(cmd, timeout).Result;
    #endregion

    #region async
    public async Task<List<TModel>> ModelListAsync<TModel>(string sql = "", int timeout = 30) where TModel : new()
    {
        log.start("ToList", sql, "");
        // If a query is empty create the query from the Model

        if (string.IsNullOrWhiteSpace(sql))
        {
            SqlQueryTranslator queryTranslator = new SqlQueryTranslator(RequiredFields);
            sql = queryTranslator.SetQuery<TModel>();
        }
        else
        {
            QHelpers.CheckQuery(sql);
        }

        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = timeout;
        List<TModel> result = await ModelListAsync<TModel>(cmd, timeout);
        cmd.Dispose();
        return result;

    }

    /// <summary>
    /// Devuelve datos de la consulta
    /// </summary>
    /// <param name="cmd">Comando a ejecutar</param>
    /// <param name="timeout">time out in seconds</param>
    /// <returns>
    /// Devuelve los datos de la consulta en un List<T>
    /// Si hay error devuelve el mensaje con el error
    /// </returns>
    public Task<List<TModel>> ModelListAsync<TModel>(SqlCommand cmd, int timeout = 30) where TModel : new()
    {
        log.start("ToList", cmd.CommandText, "");
        try
        {
            using SqlConnection cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandTimeout = timeout;
            cmd.Connection.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            List<TModel> result = new List<TModel>();
            if (dr is not null)
            {
                bool canRead = dr.Read();
                if (canRead)
                {
                    Type model = typeof(TModel);
                    int tableCount = 0;
                    TableName table = new TableName(model.Name, $"t{tableCount}", string.Empty, InnerDirection.NONE, string.Empty, string.Empty, model.Name);
                    List<TableName> tableNamesBK = new List<TableName>();
                    TableNamesHelper tableNames = new TableNamesHelper();
                    tableNames.AddTableNames<TModel>();
                    tableNamesBK = new List<TableName>(tableNames.TableNames);

                    ReadOnlyCollection<DbColumn> columnNames = dr.GetColumnSchema();
                    //ColumnsNames ch = new ColumnsNames(columnNames, TableNames);
                    InstanceModel instanceModel = new InstanceModel();

                    ColumnSqlClientToObject columnToObject = new ColumnSqlClientToObject(new ColumnsNames(columnNames, tableNamesBK), instanceModel, tableNamesBK);

                    object currentRow = new string("");
                    int t = tableNamesBK.Count;
                    do
                    {
                        bool hasList = false;
                        TModel dat = new();
                        instanceModel.InstanceProperties(dat);
                        ColumnValue columnValue = new ColumnValue(tableNamesBK, dat);
                        ColumnToObjectResponse response = new ColumnToObjectResponse(dat);

                        currentRow = dr[0];//know what is the first column asume it's the key column and no repeated
                        int i = 0;
                        do
                        {
                            response = columnToObject.SetColumnToObject(new ColumnValue(tableNamesBK, response.InUse),
                                                dr, response.InUse, tableNamesBK[i].ShortName);                    
                            if (response.IsList)
                            {
                                hasList = true;
                                object listInstance = response.PropertyListInstance;
                                string listName = response.PropertyListName;
                                //check if have some other object like a property
                                TableName tableList = tableNamesBK.Where(t => t.ShortReference == response.ActualTable).FirstOrDefault();
                                if (tableList != null)
                                {
                                    response = columnToObject.SetColumnToObject(new ColumnValue(tableNamesBK, response.PropertyListData),
                                                   dr, response.PropertyListData, tableList.ShortName);
                                }

                                listInstance.GetPropValue(listName).GetType()
                                    .GetMethod("Add").Invoke(listInstance.GetPropValue(listName),
                                                                        new[] { response.InUse });
                                response.InUse = listInstance;
                            }
                            dat = (TModel)response.InUse;
                            i++;
                            if (i >= t)
                            {
                                i = 0;
                                canRead = canRead = dr.Read();
                                if (!hasList) currentRow = -1;
                            }
                        } while (canRead && currentRow.ToString() == dr[0].ToString());

                        result.Add(dat);
                        columnValue = null;
                    } while (canRead);
                }

            }
            cmd.Connection.Close();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            log.end(cmd.CommandText, ex.ToString());
            throw;
        }
    }
    #endregion
}
