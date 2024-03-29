﻿using Database.ADO.ValueObjects;

namespace Database.ADO;

/// <summary>
/// Management of MS-SQL DataBaseWithADO
/// </summary>
public class DataBaseWithADO
{
    #region Porperties
    private string ConnectionStringBK;
    public string ConnectionString
    {
        get { return ConnectionStringBK; }
        private set
        {
            ConnectionStringBK = value;
            //asegurarse de que
            if(string.IsNullOrEmpty(ConnectionStringBK))
            {
                // crear la cadena de conexion con la base de datos por defecto
                string source = "localhost";
                string catalog = "default";
                string user = "sa";
                string pass = "0123456789";
                SetConnectionString(source, catalog, user, pass);
            }
        }
    }
    #endregion

    #region management variables  
    readonly Options Options;
    Dictionary<string, object> WhereRequired;
    private IDbLog LogService;
    #endregion

    #region Constructors

    #region For Injection
    public DataBaseWithADO(Microsoft.Extensions.Options.IOptions<DatabaseOptions> options, IDbLog loger = null)
    {
        ConnectionStringBK = options.Value.ConnectionString;
        Options = options.Value.Options;
        WhereRequired = options.Value.Params?.ToDictionary(x => x.ColumnName, x => x.Value);
        SetLogger(loger);
    }
    public DataBaseWithADO(Action<DatabaseOptions> action, IDbLog loger = null)
    {
        DatabaseOptions options = new();
        action(options);
        ConnectionStringBK = options.ConnectionString;
        Options = options.Options;
        WhereRequired = options.Params?.ToDictionary(x => x.ColumnName, x => x.Value);
        SetLogger(loger);
    }
    #endregion
    #region To instance
    public DataBaseWithADO()
    {
        SetLogger(null);
        Options = default;
        ConnectionString = string.Empty;
        WhereRequired = new Dictionary<string, object>();
    }

    public DataBaseWithADO(Options options) : this() => Options = options;

    public DataBaseWithADO(params KeyValuePair<string, object>[] requiredWhereValues) : this()
        => WhereRequired = requiredWhereValues.ToDictionary(x => x.Key, x => x.Value);

    public DataBaseWithADO(Options options, params KeyValuePair<string, object>[] requiredWhereValues) : this()
    {
        Options = options;
        WhereRequired = requiredWhereValues.ToDictionary(x => x.Key, x => x.Value);
    }

    public DataBaseWithADO(string connectionString) : this()
        => ConnectionStringBK = connectionString;
    public DataBaseWithADO(string connectionString, params KeyValuePair<string, object>[] args) : this(args)
        => ConnectionStringBK = connectionString;
    public DataBaseWithADO(string connectionString, Options options) : this(options)
        => ConnectionStringBK = connectionString;

    public DataBaseWithADO(string connectionString, Options options, params KeyValuePair<string, object>[] requiredWhereValues) : this(options, requiredWhereValues)
        => ConnectionStringBK = connectionString;

    public DataBaseWithADO(string source, string catalog, string user, string pass, params KeyValuePair<string, object>[] args) : this(args)
        => SetConnectionString(source, catalog, user, pass);
    public DataBaseWithADO(string source, string catalog, string user, string pass, Options options) : this(options)
        => SetConnectionString(source, catalog, user, pass);

    public DataBaseWithADO(string source, string catalog, string user, string pass, int poolSize, Options options, params KeyValuePair<string, object>[] args) : this(options, args)
        => SetConnectionString(source, catalog, user, pass, poolSize);
    #endregion
    #endregion

    #region setup methods                          
    public void SetConnectionString(string connectionString) =>
        ConnectionStringBK = connectionString;

    public void SetConnectionString(string source, string catalog, string username, string password, int poolSize = 100, string workstation = "", int packetSize = 4096, bool security = true)
    {
        StringBuilder strCadena = new();
        if(!string.IsNullOrEmpty(source))
        {
            strCadena.Append($"Data Source={source};Initial Catalog={catalog};Persist Security Info=");
            if(security) strCadena.Append("true;");
            else strCadena.Append("false;");
            strCadena.Append($"User ID={username};Password={password};");
            strCadena.Append($"Max Pool Size={poolSize};");
            if(!string.IsNullOrEmpty(workstation)) strCadena.Append($"workstation id={workstation};");
            strCadena.Append($"packet size={packetSize};");
        }
        ConnectionStringBK = strCadena.ToString();
    }

    public void SetLogger(IDbLog logger)
    {
        if(logger == null) LogService = new DbLogin(Options.LogOptions.LogFolder);
        else LogService = logger;
    }

    public void SetWhere(Dictionary<string, object> where)
    {
        WhereRequired = where;
    }

    public void SetWhere(string column, object value)
    {
        if(WhereRequired.ContainsKey(column))
        {
            WhereRequired[column] = value;
        }
        else
        {
            WhereRequired.Add(column, value);
        }
    }

    public object GetWhereValue(string key)
    {
        return WhereRequired.Where(k => k.Key == key).FirstOrDefault().Value;
    }

    public string SetQuery<TModel>(string indexColumn = "", int startIndex = 0, int numElements = 0)
    {
        SqlQueryTranslator queryTranslator = new SqlQueryTranslator(WhereRequired);
        return queryTranslator.SetQuery<TModel>(indexColumn, startIndex, numElements);
    }
    #endregion

    #region wrapper commands
    private Commands CreateCommands() => new(LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public object Execute(string query, int timeout = 30) => CreateCommands().Execute(query, timeout);
    public bool ExecuteCommand(string query, int timeout = 30) => CreateCommands().ExecuteCommand(query, timeout);
    public Task<object> ExecuteAsync(string query, int timeout = 30) => CreateCommands().ExecuteAsync(query, timeout);
    public Task<bool> ExecuteCommandAsync(string query, int timeout = 30) => CreateCommands().ExecuteCommandAsync(query, timeout);
    public bool ExecuteCommand(SqlCommand cmd, int timeout = 30) => CreateCommands().ExecuteCommand(cmd, timeout);
    public object Execute(SqlCommand cmd, int timeout = 30) => CreateCommands().Execute(cmd, timeout);
    public Task<bool> ExecuteCommandAsync(SqlCommand cmd, int timeout = 30) => CreateCommands().ExecuteCommandAsync(cmd, timeout);
    public Task<object> ExecuteAsync(SqlCommand cmd, int timeout = 30) => CreateCommands().ExecuteAsync(cmd, timeout);
    #endregion

    #region wrapper delete                                  
    private DeleteRows CreateDeleteRows() => new(LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public bool DeleteRow(string table, string indexColumn, int index) => CreateDeleteRows().DeleteRow(table, indexColumn, index);
    public bool DeleteRow(string table, string indexColumn, string index) => CreateDeleteRows().DeleteRow(table, indexColumn, index);
    public bool DeleteRow(string table, string[] indexColumn, object[] index) => CreateDeleteRows().DeleteRow(table, indexColumn, index);
    public bool DeleteRow(string table, string[] indexColumn, int[] index) => CreateDeleteRows().DeleteRow(table, indexColumn, index);
    public Task<bool> DeleteRowAsync(string table, string indexColumn, int index) => CreateDeleteRows().DeleteRowAsync(table, indexColumn, index);
    public Task<bool> DeleteRowAsync(string table, string indexColumn, string index) => CreateDeleteRows().DeleteRowAsync(table, indexColumn, index);
    public Task<bool> DeleteRowAsync(string table, string[] indexColumn, object[] index) => CreateDeleteRows().DeleteRowAsync(table, indexColumn, index);
    public Task<bool> DeleteRowAsync(string table, string[] indexColumn, int[] index) => CreateDeleteRows().DeleteRowAsync(table, indexColumn, index);
    #endregion

    #region wrapper Images                                  
    private Images CreateImages() => new(LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public bool UpdateImage(string table, string indexColumn, string index, string imageColumn, string image) => CreateImages().UpdateImage(table, indexColumn, index, imageColumn, image);
    public bool UpdateImage(string table, string indexColumn, string index, string imageColumn, byte[] image) => CreateImages().UpdateImage(table, indexColumn, index, imageColumn, image);
    public bool InsertImage(string table, string imageColumn, string image) => CreateImages().InsertImage(table, imageColumn, image);
    public bool InsertImage(string table, string imageColumn, byte[] image) => CreateImages().InsertImage(table, imageColumn, image);
    public bool InsertImage(string table, string indexColumn, string index, string imageColumn, string image) => CreateImages().InsertImage(table, indexColumn, index, imageColumn, image);
    public bool InsertImage(string table, string indexColumn, string index, string imageColumn, byte[] image) => CreateImages().InsertImage(table, indexColumn, index, imageColumn, image);
    public Task<bool> UpdateImageAsync(string table, string indexColumn, string index, string imageColumn, string image) => CreateImages().UpdateImageAsync(table, indexColumn, index, imageColumn, image);
    public Task<bool> UpdateImageAsync(string table, string indexColumn, string index, string imageColumn, byte[] image) => CreateImages().UpdateImageAsync(table, indexColumn, index, imageColumn, image);
    public Task<bool> InsertImageAsync(string table, string imageColumn, string image) => CreateImages().InsertImageAsync(table, imageColumn, image);
    public Task<bool> InsertImageAsync(string table, string imageColumn, byte[] image) => CreateImages().InsertImageAsync(table, imageColumn, image);
    public Task<bool> InsertImageAsync(string table, string indexColumn, string index, string imageColumn, string image) => CreateImages().InsertImageAsync(table, indexColumn, index, imageColumn, image);
    public Task<bool> InsertImageAsync(string table, string indexColumn, string index, string imageColumn, byte[] image) => CreateImages().InsertImageAsync(table, indexColumn, index, imageColumn, image);

    #endregion

    #region wrapper Update                                  
    private Update CreateUpdate() => new(LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public bool UpdateColumn(string table, string colName, object colValue, string indexColumn, int index) => CreateUpdate().UpdateColumn(table, colName, colValue, indexColumn, index);
    public bool UpdateColumn(string table, string colName, object colValue, string indexColumn, object index) => CreateUpdate().UpdateColumn(table, colName, colValue, indexColumn, index);
    public bool UpdateColumn(string table, string[] colName, object[] colValue, string indexColumn, int index) => CreateUpdate().UpdateColumn(table, colName, colValue, indexColumn, index);
    public bool UpdateColumn(string table, string[] colName, object[] colValue, string indexColumn, object index) => CreateUpdate().UpdateColumn(table, colName, colValue, indexColumn, index);
    public bool UpdateColumn(string table, string[] colName, object[] colValue, string[] indexColumn, object[] index) => CreateUpdate().UpdateColumn(table, colName, colValue, indexColumn, index);
    public Task<bool> UpdateColumnAsync(string table, string colName, object colValue, string indexColumn, int index) => CreateUpdate().UpdateColumnAsync(table, colName, colValue, indexColumn, index);
    public Task<bool> UpdateColumnAsync(string table, string colName, object colValue, string indexColumn, object index) => CreateUpdate().UpdateColumnAsync(table, colName, colValue, indexColumn, index);
    public Task<bool> UpdateColumnAsync(string table, string[] colName, object[] colValue, string indexColumn, int index) => CreateUpdate().UpdateColumnAsync(table, colName, colValue, indexColumn, index);
    public Task<bool> UpdateColumnAsync(string table, string[] colName, object[] colValue, string indexColumn, object index) => CreateUpdate().UpdateColumnAsync(table, colName, colValue, indexColumn, index);
    public Task<bool> UpdateColumnAsync(string table, string[] colName, object[] colValue, string[] indexColumn, object[] index) => CreateUpdate().UpdateColumnAsync(table, colName, colValue, indexColumn, index);

    #endregion

    #region wrapper Insert                                  
    private Insert CreateInsert() => new(LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public bool InsertInDB(string table, string colName, object colValue) => CreateInsert().InsertInDB(table, colName, colValue);
    public bool InsertInDB(string table, string[] colName, object[] colValue) => CreateInsert().InsertInDB(table, colName, colValue);
    public int InsertInDB(string table, string[] colName, object[] colValue, bool returnScope) => CreateInsert().InsertInDB(table, colName, colValue, returnScope);
    public Task<int> InsertInDBAsync(string table, string[] colName, object[] colValue, bool returnScope) => CreateInsert().InsertInDBAsync(table, colName, colValue, returnScope);

    #endregion

    #region wrapper Management                                  
    private Management CreateManagement() => new(LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public bool HasRows(string sql, int timeout = 30) => CreateManagement().HasRows(sql, timeout);
    public Task<bool> HasRowsAsync(string sql, int timeout = 30) => CreateManagement().HasRowsAsync(sql, timeout);
    public int GetNewId(string Tabla) => CreateManagement().GetNewId(Tabla);
    public int GetNewId(string Tabla, string col) => CreateManagement().GetNewId(Tabla, col);
    public string GetCol(string sql, string colSQL, int timeOut = 30) => GetColAync(sql, colSQL, timeOut).Result;
    public string GetCol(string sql, int colSQL, int timeOut = 30) => GetColAync(sql, colSQL, timeOut).Result;
    public Task<string> GetColAync(string sql, string colSQL, int timeOut = 30) => CreateManagement().GetColAync(sql, colSQL, timeOut);
    public Task<string> GetColAync(string sql, int colSQL, int timeOut = 30) => CreateManagement().GetColAync(sql, colSQL, timeOut);

    #endregion

    #region wrapper QueryDataSet                                  
    private QueryDataSet CreateQueryDataSet() => new(WhereRequired, LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public DataSet GetDataSet<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) => CreateQueryDataSet().GetDataSet<TModel>(timeout, indexColumn, startIndex, numElements);
    public DataSet GetDataSet<TModel>(int timeout = 30) => CreateQueryDataSet().GetDataSet<TModel>(timeout);
    public DataSet GetDataSet(string sql, int timeout = 30) => CreateQueryDataSet().GetDataSet(sql, timeout);
    public Task<DataSet> GetDataSetAsync<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) => CreateQueryDataSet().GetDataSetAsync<TModel>(timeout, indexColumn, startIndex, numElements);
    public Task<DataSet> GetDataSetAsync<TModel>(int timeout = 30) => CreateQueryDataSet().GetDataSetAsync<TModel>(timeout);
    public Task<DataSet> GetDataSetAsync(string sql, int timeout = 30) => CreateQueryDataSet().GetDataSetAsync(sql, timeout);
    #endregion     

    #region wrapper QueryDataTable                                  
    private QueryDataTable CreatQueryDataTable() => new(WhereRequired, LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public DataTable GetDataTable<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) => CreatQueryDataTable().GetDataTable<TModel>(timeout, indexColumn, startIndex, numElements);
    public DataTable GetDataTable<TModel>(int timeout = 30) => CreatQueryDataTable().GetDataTable<TModel>(timeout);
    public DataTable GetDataTable(string sql, int timeout = 30) => CreatQueryDataTable().GetDataTable(sql, timeout);
    public Task<DataTable> GetDataTableAsync<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) => CreatQueryDataTable().GetDataTableAsync<TModel>(timeout, indexColumn, startIndex, numElements);
    public Task<DataTable> GetDataTableAsync<TModel>(int timeout = 30) => CreatQueryDataTable().GetDataTableAsync<TModel>(timeout);
    public Task<DataTable> GetDataTableAsync(string sql, int timeout = 30) => CreatQueryDataTable().GetDataTableAsync(sql, timeout);
    #endregion   

    #region wrapper QueryDataView                                  
    private QueryDataView CreatQueryDataView() => new(WhereRequired, LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public DataView GetDataView<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) => CreatQueryDataView().GetDataView<TModel>(timeout, indexColumn, startIndex, numElements);
    public DataView GetDataView<TModel>(int timeout = 30) => CreatQueryDataView().GetDataView<TModel>(timeout);
    public DataView GetDataView(string sql, int timeout = 30) => CreatQueryDataView().GetDataView(sql, timeout);
    public Task<DataView> DataViewAsync<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) => CreatQueryDataView().DataViewAsync<TModel>(timeout, indexColumn, startIndex, numElements);
    public Task<DataView> DataViewAsync<TModel>(int timeout = 30) => CreatQueryDataView().DataViewAsync<TModel>(timeout);
    public Task<DataView> DataViewAsync(string sql, int timeout = 30) => CreatQueryDataView().DataViewAsync(sql, timeout);
    #endregion 

    #region wrapper QueryListWithModel                                  
    private QueryListWithModel CreatQueryListWithModel() => new(WhereRequired, LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public List<TModel> ModelList<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) where TModel : new() => CreatQueryListWithModel().ModelList<TModel>("", timeout, indexColumn, startIndex, numElements);
    public List<TModel> ModelList<TModel>(string sql = "", int timeout = 30) where TModel : new() => CreatQueryListWithModel().ModelList<TModel>(sql, timeout);
    public List<TModel> ModelList<TModel>(SqlCommand cmd, int timeout = 30) where TModel : new() => CreatQueryListWithModel().ModelList<TModel>(cmd, timeout);
    public Task<List<TModel>> ModelListAsync<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) where TModel : new() => CreatQueryListWithModel().ModelListAsync<TModel>("", timeout, indexColumn, startIndex, numElements);
    public Task<List<TModel>> ModelListAsync<TModel>(string sql = "", int timeout = 30) where TModel : new() => CreatQueryListWithModel().ModelListAsync<TModel>(sql, timeout);
    public Task<List<TModel>> ModelListAsync<TModel>(SqlCommand cmd, int timeout = 30) where TModel : new() => CreatQueryListWithModel().ModelListAsync<TModel>(cmd, timeout);
    public List<TModel> List<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) where TModel : new() => ModelList<TModel>(indexColumn, startIndex, numElements, timeout);
    public List<TModel> List<TModel>(string sql = "", int timeout = 30) where TModel : new() => ModelList<TModel>(sql, timeout);
    public List<TModel> List<TModel>(SqlCommand cmd, int timeout = 30) where TModel : new() => ModelList<TModel>(cmd, timeout);
    public Task<List<TModel>> ListAsync<TModel>(string indexColumn, int startIndex, int numElements, int timeout = 30) where TModel : new() => ModelListAsync<TModel>(indexColumn, startIndex, numElements, timeout);
    public Task<List<TModel>> ListAsync<TModel>(string sql = "", int timeout = 30) where TModel : new() => ModelListAsync<TModel>(sql, timeout);
    public Task<List<TModel>> ListAsync<TModel>(SqlCommand cmd, int timeout = 30) where TModel : new() => ModelListAsync<TModel>(cmd, timeout);
    #endregion   

    #region wrapper QueryWithModel                                  
    private QueryWithModel CreatQueryWithModel() => new(WhereRequired, LogService, Options.LogOptions.LogResults, Options.EnableSqlInjectionControl, Options.EnableCharControl, ConnectionStringBK);
    public TModel Get<TModel>(string sql = "", int timeout = 30) where TModel : new() => CreatQueryWithModel().Get<TModel>(sql, timeout);
    public Task<TModel> GetAsync<TModel>(string sql = "", int timeout = 30) where TModel : new() => CreatQueryWithModel().GetAsync<TModel>(sql, timeout);
    #endregion

    #region wrapper bulk actions 
    public async Task<bool> BulkCopyAsync(DataTable dt, string destinationTableName, int timeout = 660)
    {
        BulkCommands cbulkCommand = new BulkCommands(LogService, Options.LogOptions.LogResults, ConnectionStringBK);
        return await cbulkCommand.BulkCopyAsync(dt, destinationTableName, timeout);
    }
    #endregion
}