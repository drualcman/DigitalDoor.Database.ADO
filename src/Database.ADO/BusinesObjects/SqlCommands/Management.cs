namespace Database.ADO.BusinesObjects.SqlCommands;

/// <summary>
/// Management of MS-SQL DataBases
/// </summary>
internal class Management : SqlBaseCommands
{
    readonly Commands Commands;
    public Management(IDbLog dbLog, bool logResults, bool databaseControl, bool charControl, string connectionString)
        : base(dbLog, logResults, databaseControl, charControl, connectionString)
        => Commands = new(dbLog, logResults, databaseControl, charControl, connectionString);


    public async Task<string> GetColAync(string sql, string colSQL, int timeOut = 30)
    {
        log.start("GetColAync(sql, colSQL, timeOut)", sql, colSQL.ToString() + ", " + timeOut.ToString());
        string datoRetorno = string.Empty;
        try
        {
            QHelpers.CheckQuery(sql);
            using SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeOut;
            using SqlDataReader dr = await Commands.ReaderAsync(cmd, timeOut);
            if (dr.HasRows)
            {
                if (dr.Read())
                    datoRetorno = dr[colSQL].ToString();
                else
                    datoRetorno = string.Empty;
            }
            else
                datoRetorno = string.Empty;
            dr.Close();
        }
        catch (Exception exConexion)
        {
            log.end(null, exConexion.ToString());
            throw;
        }
        if (LogResults)
            log.end(datoRetorno);
        return datoRetorno;
    }

    public async Task<string> GetColAync(string sql, int colSQL, int timeOut = 30)
    {
        log.start("GetColAync(sql, colSQL, timeOut)", sql, colSQL.ToString() + ", " + timeOut.ToString());
        string datoRetorno = string.Empty;
        try
        {
            QHelpers.CheckQuery(sql);
            using SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeOut;
            using SqlDataReader dr = await Commands.ReaderAsync(cmd);         // ejecutar el comando SQL
            if (dr.HasRows)
            {
                if (dr.Read())                                      // leer los datos
                    datoRetorno = dr[colSQL].ToString();      // obtener el campo deseado
                else
                    datoRetorno = string.Empty;
            }
            else
                datoRetorno = string.Empty;
            dr.Close();                                     // cerrar la consulta

        }
        catch (Exception exConexion)
        {
            log.end(null, exConexion.ToString());
            throw;
        }
        if (LogResults)
            log.end(datoRetorno);

        return datoRetorno;
    }

    public bool HasRows(string sql, int timeout = 30)
    {
        log.start("ExisteEnDDBB(sql)", sql, "");
        bool retorno = false;
        try
        {
            QHelpers.CheckQuery(sql);
            using SqlCommand command = new SqlCommand();
            command.CommandText = sql;
            command.CommandTimeout = timeout;
            using SqlDataReader dr = Commands.Reader(command);
            retorno = dr.HasRows;
            dr.Close();                                     // cerrar la consulta
        }
        catch (Exception ex)
        {
            log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            log.end(retorno);
        return retorno;
    }

    public async Task<bool> HasRowsAsync(string sql, int timeout = 30)
    {

        log.start("ExisteEnDDBB(sql)", sql, "");
        bool retorno = false;
        try
        {
            using SqlCommand command = new SqlCommand();
            command.CommandTimeout = timeout;
            using SqlDataReader dr = await Commands.ReaderAsync(command);
            retorno = dr.HasRows;
            dr.Close();
        }
        catch (Exception ex)
        {
            log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            log.end(retorno);
        return retorno;
    }

    public int GetNewId(string Tabla)
    {
        string sql = "SELECT IDENT_CURRENT('" + Tabla + "')";

        log.start("ObtenerNuevoId(tabla)", sql, Tabla);

        int newId = -1;

        try
        {
            QHelpers.CheckQuery(sql);
            string dato = GetColAync(sql, 0).Result;
            newId = Convert.ToInt32(dato);
            newId++;
        }
        catch (Exception ex)
        {
            log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            log.end(newId);
        return newId;
    }

    public int GetNewId(string Tabla, string col)
    {
        string sql = "SELECT TOP 1 " + col + " FROM " + Tabla +
            " ORDER BY " + col + " DESC";
        log.start("ObtenerNuevoId(tabla, col)", sql, Tabla + "," + col);
        int newId = -1;
        try
        {
            QHelpers.CheckQuery(sql);
            string dato = GetColAync(sql, 0).Result;
            if (string.IsNullOrEmpty(dato)) newId = 0;
            else newId = Convert.ToInt32(dato);
            newId++;
        }
        catch (Exception ex)
        {
            log.end(sql, ex.ToString());
            throw;
        }
        if (LogResults)
            log.end(newId);
        return newId;
    }
}