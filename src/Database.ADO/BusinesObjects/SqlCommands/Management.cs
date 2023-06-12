namespace Database.ADO.BusinesObjects.SqlCommands;

/// <summary>
/// Management of MS-SQL DataBases
/// </summary>
internal sealed class Management : SqlBaseCommands
{
    readonly QueryDataTable Commands;
    public Management(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(dbLog, logResults, databaseControl, charControl, connectionString)
        => Commands = new(new Dictionary<string, object>(), dbLog, logResults, databaseControl, charControl, connectionString);

    public async Task<string> GetColAync(string sql, string colSQL, int timeOut = 30)
    {
        Log.start("GetColAync(sql, colSQL, timeOut)", sql, colSQL.ToString() + ", " + timeOut.ToString());
        string datoRetorno = string.Empty;
        try
        {
            using DataTable dt = await Commands.GetDataTableAsync(sql, timeOut);
            if (dt.Rows.Count > 0)
            {
                datoRetorno = dt.Rows[0][colSQL].ToString();
            }
            else
                datoRetorno = string.Empty;
        }
        catch (Exception exConexion)
        {
            Log.end(null, exConexion.ToString());
            throw;
        }
        if (LogResults)
            Log.end(datoRetorno);
        return datoRetorno;
    }

    public async Task<string> GetColAync(string sql, int colSQL, int timeOut = 30)
    {
        Log.start("GetColAync(sql, colSQL, timeOut)", sql, colSQL.ToString() + ", " + timeOut.ToString());
        string datoRetorno = string.Empty;
        try
        {               
            using DataTable dt = await Commands.GetDataTableAsync(sql, timeOut);
            if (dt.Rows.Count > 0)
            {
                datoRetorno = dt.Rows[0][colSQL].ToString();
            }
            else
                datoRetorno = string.Empty;
        }
        catch (Exception exConexion)
        {
            Log.end(null, exConexion.ToString());
            throw;
        }
        if (LogResults)
            Log.end(datoRetorno);

        return datoRetorno;
    }

    public bool HasRows(string sql, int timeout = 30)
    {
        Log.start("ExisteEnDDBB(sql)", sql, "");
        bool retorno = false;
        try
        {
            using DataTable dt = Commands.GetDataTable(sql, timeout);
            retorno = dt.Rows.Count > 0;
        }
        catch (Exception ex)
        {
            Log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            Log.end(retorno);
        return retorno;
    }

    public async Task<bool> HasRowsAsync(string sql, int timeout = 30)
    {

        Log.start("ExisteEnDDBB(sql)", sql, "");
        bool retorno = false;
        try
        {
            using DataTable dt = await Commands.GetDataTableAsync(sql, timeout);
            retorno = dt.Rows.Count > 0;
        }
        catch (Exception ex)
        {
            Log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            Log.end(retorno);
        return retorno;
    }

    public int GetNewId(string Tabla)
    {
        string sql = "SELECT IDENT_CURRENT('" + Tabla + "')";

        Log.start("ObtenerNuevoId(tabla)", sql, Tabla);

        int newId = -1;

        try
        {
            string dato = GetColAync(sql, 0).Result;
            newId = Convert.ToInt32(dato);
            newId++;
        }
        catch (Exception ex)
        {
            Log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            Log.end(newId);
        return newId;
    }

    public int GetNewId(string Tabla, string col)
    {
        string sql = "SELECT TOP 1 " + col + " FROM " + Tabla +
            " ORDER BY " + col + " DESC";
        Log.start("ObtenerNuevoId(tabla, col)", sql, Tabla + "," + col);
        int newId = -1;
        try
        {
            string dato = GetColAync(sql, 0).Result;
            if (string.IsNullOrEmpty(dato)) newId = 0;
            else newId = Convert.ToInt32(dato);
            newId++;
        }
        catch (Exception ex)
        {
            Log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            Log.end(newId);
        return newId;
    }
}