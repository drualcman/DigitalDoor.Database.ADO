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
        if(sql.ToUpper().IndexOf("UPDATE ") < 0)
        {
            if(sql.ToUpper().IndexOf("INSERT ") < 0)
            {
                if(sql.ToUpper().IndexOf("DELETE ") < 0)
                {
                    if(sql.ToUpper().IndexOf("EXEC ") < 0)
                    {
                        if(sql.ToUpper().IndexOf("DROP ") < 0)
                        {
                            if(sql.ToUpper().IndexOf("ALTER ") < 0)
                            {
                                if(sql.ToUpper().IndexOf("CREATE ") < 0)
                                {
                                    string err = "La cadena debe ser: " + "\r\n" +
                                        "UPDATE < tabla > SET < campo=valor >" + "\r\n" +
                                        "INSERT INTO < tabla > VALUES < campo=valor >" + "\r\n" +
                                        "DELETE < tabla > WHERE < condicion >" + "\r\n" +
                                        "EXEC  < Storage Proccess > < Varaibles >" + "\r\n" +
                                        "CREATE TABLE" + "\r\n" +
                                        "DROP TABLE/PROCEDURE/FUNCTION < tabla >" + "\r\n" +
                                        "ALTER TABLE < tabla > < definicion >" + "\r\n" +
                                        "SQL: " + sql;
                                    Log.end(null, err);

                                    throw new ArgumentException(err);
                                }
                            }
                        }
                    }
                }
            }
        } 
        ChecInjection(sql);
    }

    private void ChecInjection(string query)
    {
        if(WithoutSchema(query))
        {
            // no permitir comentarios ni algunas instrucciones maliciosas
            if(query.IndexOf("--") > -1)
            {
                Log.end(null, "Comments not allowed");

                throw new ArgumentException("Comments not allowed. SQL: " + query);
            }
            else if(query.ToUpper().IndexOf("DROP TABLE ") > -1)
            {
                Log.end(null, "Must be SELECT fields FROM table, not DROP or other commands...");

                throw new ArgumentException("Must be SELECT fields FROM table, not DROP or other commands... SQL: " + query);
            }
            else if(query.ToUpper().IndexOf("DROP PROCEDURE ") > -1)
            {
                Log.end(null, "Must be SELECT fields FROM table, not DROP or other commands...");

                throw new ArgumentException("Must be SELECT fields FROM table, not DROP or other commands... SQL: " + query);
            }
            else if(query.ToUpper().IndexOf("DROP FUNCTION ") > -1)
            {
                Log.end(null, "LMust be SELECT fields FROM table, not DROP or other commands...");

                throw new ArgumentException("Must be SELECT fields FROM table, not DROP or other commands... SQL: " + query);
            }
        }
        else
        {
            Log.end(null, "SQL control unsuccessful.");
            throw new ArgumentException("Query must be SELECT fields FROM table / EXEC Storage Process and variables. SQL: " + query);
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
    #endregion
}
