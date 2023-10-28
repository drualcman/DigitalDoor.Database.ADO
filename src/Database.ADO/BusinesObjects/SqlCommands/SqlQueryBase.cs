namespace Database.ADO.BusinesObjects.SqlCommands;
internal abstract class SqlQueryBase : SqlBaseCommands
{
    protected readonly Dictionary<string, object> RequiredFields;

    public SqlQueryBase(Dictionary<string, object> requiredFields, IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(dbLog, logResults, databaseControl, charControl, connectionString) =>
        RequiredFields = requiredFields;

    protected string SetQuery<TModel>(string indexColumn = "", int pageNumber = 0, int numElements = 0)
    {
        SqlQueryTranslator queryTranslator = new SqlQueryTranslator(RequiredFields);
        return queryTranslator.SetQuery<TModel>(indexColumn, pageNumber, numElements);
    }
}
