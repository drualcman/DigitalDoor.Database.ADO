namespace Database.ADO.BusinesObjects.Converters;

internal class DataSetConverter
{
    internal static string ToJson(DataSet ds)
        => ds.ToJson();
}
