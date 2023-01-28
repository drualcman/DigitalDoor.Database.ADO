namespace Database.ADO.BusinesObjects.Helpers;

internal class QueryHelpers
{
    readonly bool DatabaseControl;
    readonly bool CharControl;
    readonly IDbLog Log;

    public QueryHelpers(bool databaseControl, bool charControl, IDbLog log)
    {
        DatabaseControl = databaseControl;
        CharControl = charControl;
        Log = log;
    }

    public static string CleanSqlDataColumns(string input)
    {
        string pattern = "\\[t[0-9].";
        string replacement = "[";
        return Regex.Replace(input, pattern, replacement);
    }

    #region security     
    public void CheckQuery(string sql)
    {
        ChecInjection(sql);
        if(!sql.ToUpper().StartsWith("UPDATE "))
        {
            if(!sql.ToUpper().StartsWith("INSERT "))
            {
                if(!sql.ToUpper().StartsWith("DELETE "))
                {
                    if(!sql.ToUpper().StartsWith("EXEC "))
                    {
                        if(!sql.ToUpper().StartsWith("DROP "))
                        {
                            if(!sql.ToUpper().StartsWith("ALTER "))
                            {
                                if(!sql.ToUpper().StartsWith("CREATE "))
                                {
                                    if(!sql.ToUpper().StartsWith("SELECT "))
                                        ThrowException(sql, "Check your query");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void ChecInjection(string query)
    {
        if(WithoutSchema(query))
        {
            // no permitir comentarios ni algunas instrucciones maliciosas
            if(query.IndexOf("--") > -1)
            {
                ThrowException(query, "Comments not allowed. SQL: ");
            }
            else if(query.ToUpper().IndexOf("DROP TABLE ") > -1)
            {
                ThrowException(query, "Must be SELECT fields FROM table, not DROP or other commands... SQL: ");
            }
            else if(query.ToUpper().IndexOf("DROP PROCEDURE ") > -1)
            {
                ThrowException(query, "Must be SELECT fields FROM table, not DROP or other commands... SQL: ");
            }
            else if(query.ToUpper().IndexOf("DROP FUNCTION ") > -1)
            {
                ThrowException(query, "Must be SELECT fields FROM table, not DROP or other commands...");
            }
        }
        else
        {
            ThrowException(query, "Query must be SELECT fields FROM table / EXEC Storage Process and variables ");
        }
    }

    private bool WithoutSchema(string query)
    {
        bool resultado;
        if(this.DatabaseControl == true)
        {
            if(!string.IsNullOrEmpty(query))
            {
                if(query.ToUpper().IndexOf("INFORMATION_SCHEMA") >= 0)
                    resultado = false;
                else if(query.ToLower().IndexOf("sysobjects") >= 0)
                    resultado = false;
                else if(query.ToLower().IndexOf("syscolumns") >= 0)
                    resultado = false;
                else if(query.ToUpper().IndexOf("BENCHMARK(") >= 0)
                    resultado = false;
                else if(this.CharControl == true)
                {
                    int tiene = query.ToLower().IndexOf("chr(");
                    if(tiene >= 0)
                        resultado = false;
                    else
                        resultado = true;
                }
                else
                    resultado = true;
            }
            else
                resultado = false;
        }
        else resultado = true;
        return resultado;
    }

    private void ThrowException(string sql, string aditionalMessage = "")
    {
        string err = $"{aditionalMessage}\r\nQuery must be: \r\n" +
                                        "UPDATE < table > SET < column=valuer >" + "\r\n" +
                                        "INSERT INTO < table > VALUES < column=value >" + "\r\n" +
                                        "DELETE < table > WHERE < condicion >" + "\r\n" +
                                        "EXEC  < Storage Proccess > < variables >" + "\r\n" +
                                        "CREATE TABLE" + "\r\n" +
                                        "DROP TABLE/PROCEDURE/FUNCTION < table >" + "\r\n" +
                                        "ALTER TABLE < table > < definicion >" + "\r\n" +
                                        "SQL: " + sql;
        Log.end(null, err);
        throw new ArgumentException(err);
    }
    #endregion
}
