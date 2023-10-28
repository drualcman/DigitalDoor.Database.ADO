namespace Database.ADO.ValueObjects;
public class DatabaseOptions
{
    public const string SectionName = "DatabaseOptions";
    public string ConnectionString { get; set; }
    public Options Options { get; set; }
    public IEnumerable<ParamValue>  Params { get; set; }
}
