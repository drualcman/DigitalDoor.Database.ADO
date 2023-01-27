namespace Database.ADO.BusinesObjects.Logs;

public class Loggin
{
    public string date { get; set; }
    public string starttime { get; set; }
    public string function { get; set; }
    public string sql { get; set; }
    public string vars { get; set; }
    public string endtime { get; set; }
    public string user { get; set; }
    public string error { get; set; }
    public string info { get; set; }
    public string LogFile { get; set; }
    public string LogFolder { get; set; }

    public Loggin()
    {
        LogFolder = "";
        LogFile = DateTime.Today.ToShortDateString().Replace("/", "");
        ConfigLog();
    }

    public Loggin(string file)
    {
        LogFolder = "";
        LogFile = file;
        ConfigLog();
    }

    public Loggin(string folder, string file)
    {
        LogFolder = folder;
        LogFile = file;
        ConfigLog();
    }

    public void ConfigLog()
    {
        date = DateTime.Today.ToShortDateString();
        starttime = DateTime.Now.ToShortTimeString();
        endtime = DateTime.Now.ToShortTimeString();
        sql = string.Empty;
        function = string.Empty;
        vars = string.Empty;
        error = string.Empty;
        //comprobar que el nombre de archivo tiene extension
        if (string.IsNullOrEmpty(Path.GetExtension(LogFile))) LogFile += ".log";
    }

    private void writeLog()
    {
        try
        {
            const string tag = "|";
            string log = Environment.NewLine + date + tag + starttime + tag + function +
                            tag + (string.IsNullOrEmpty(sql) ? "" : sql.Replace(Environment.NewLine, " ")) +
                            tag + vars + tag + endtime + tag + user +
                            tag + (string.IsNullOrEmpty(error) ? "" : error.Replace(Environment.NewLine, " ")) +
                            tag + info;

            string file = FilesHelpers.CheckFolder(LogFolder) + LogFile;
            if (File.Exists(file))
            {
                //append to actual log
                using StreamWriter z_varocioStreamWriter = new StreamWriter(file, true, Encoding.UTF8);
                z_varocioStreamWriter.Write(log);
                z_varocioStreamWriter.Close();
            }
            else
            {
                log = "DATE" + tag + "Start Time" + tag + "Function" + tag + "SQL" +
                        tag + "Variables" + tag + "End Time" + tag + "USER" +
                        tag + "Error Trace" + tag + "Info" + tag + log;
                FilesHelpers.SaveStringToFile(LogFile, log, LogFolder);
            }
        }
        catch
        {
            return;
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
        string _result;
        try
        {
            _result = Result.ToString();
        }
        catch
        {
            _result = string.Empty;
        }
        end(_result, string.Empty);
    }

    public void end(object Result, object Err)
    {
        string _result;
        string _err;
        try
        {
            _result = Result.ToString();
        }
        catch
        {
            _result = string.Empty;
        }
        try
        {
            _err = Err.ToString();
        }
        catch
        {
            _err = string.Empty;
        }
        end(_result, _err);
    }

    public void end(string Result, string Err)
    {
        endtime = DateTime.Now.ToShortTimeString();
        error = Err;
        info = Result;
        writeLog();
    }
}