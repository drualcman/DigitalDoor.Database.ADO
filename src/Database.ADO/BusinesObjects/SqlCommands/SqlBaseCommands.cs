namespace Database.ADO.BusinesObjects.SqlCommands;
internal abstract class SqlBaseCommands
{
    protected readonly IDbLog Log;
    protected readonly bool LogResults;
    protected readonly string ConnectionString;
    protected readonly QueryHelpers QHelpers;

    public SqlBaseCommands(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
    {
        Log = dbLog;
        LogResults = logResults;
        ConnectionString = connectionString;
        QHelpers = new QueryHelpers(databaseControl, charControl, dbLog);
    }
}
