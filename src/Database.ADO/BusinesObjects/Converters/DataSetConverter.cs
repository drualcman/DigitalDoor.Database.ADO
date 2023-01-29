namespace Database.ADO.BusinesObjects.Converters;

public class DataSetConverter
{
    public static string ToJson(DataSet ds)
        => ds.ToJson();
}
