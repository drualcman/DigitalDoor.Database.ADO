namespace Database.ADO.BusinesObjects.Logs;

internal sealed class DbLogin : IDbLog
{
    public string date { get; set; }
    public string starttime { get; set; }
    public string function { get; set; }
    public string sql { get; set; }
    public string vars { get; set; }
    public string endtime { get; set; }
    public object result { get; set; }
    public string error { get; set; }
    public string folder { get; set; }

    private void writeLog()
    {
        try
        {
            Task writeLog = Task.Run(() =>
            {
                Loggin Log = new Loggin(folder, DateTime.Today.ToShortDateString().Replace("/", "") + ".log");
                Log.date = date;
                Log.starttime = starttime;
                Log.user = "Database.ADO";
                Log.function = function;
                Log.sql = sql;
                Log.vars = vars;
                Log.end(result, error);
            });
        }
        catch
        {
            return;
        }
    }

    public DbLogin(string Folder)
    {
        date = DateTime.Today.ToShortDateString();
        starttime = DateTime.Now.ToShortTimeString();
        endtime = DateTime.Now.ToShortTimeString();
        sql = string.Empty;
        function = string.Empty;
        vars = string.Empty;
        result = false;
        error = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(Folder))
            {
                folder = FilesHelpers.CheckFolder(Folder);
            }
            else
                folder = string.Empty;
        }
        catch
        {
            folder = string.Empty;
        }
    }

    public void start(string Function, string SQL, string Vars)
    {
        function = Function;
        sql = SQL;
        vars = Vars;
    }

    public void register(string Function)
    {
        register(Function, "", "", "");
    }

    public void register(string Function, string SQL)
    {
        register(Function, SQL, "", "");
    }

    public void register(string Function, string SQL, string Vars)
    {
        register(Function, SQL, Vars, "");
    }

    public void register(string Function, string SQL, string Vars, string info)
    {
        starttime = DateTime.Now.ToString();
        function = Function;
        sql = SQL;
        vars = Vars;
        end(info);
    }

    public void end(string Result)
    {
        end(Result, string.Empty);
    }

    public void end(object Result)
    {
        end(Result, string.Empty);
    }

    public void end(object Result, object Err)
    {
        string _err;
        try
        {
            _err = Err.ToString();
        }
        catch
        {
            _err = string.Empty;
        }
        end(Result, _err);
    }


    public void end(object Result, string Err)
    {
        endtime = DateTime.Now.ToShortTimeString();
        result = Result.ToJson();
        error = Err;
        writeLog();
    }
}

