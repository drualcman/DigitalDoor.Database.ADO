namespace Database.ADO.BusinesObjects.Converters;

internal class DataTableConverter
{
    #region methods
    /// <summary>
    /// get aDataTable from Stream Data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="separator"></param>
    internal static DataTable FromStream(Stream data, char separator)
    {
        DataTable dt = new DataTable();
        return dt.FromStream(data, separator);
    }


    /// <summary>
    /// get aDataTable from Stream Data
    /// </summary>
    /// <param name="data"></param>
    internal static DataTable FromJson(string data)
    {
        DataTable dt = new DataTable();
        return dt.FromJson(data);
    }

    /// <summary>
    /// Convert DataTable to Json
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static string ToJson(DataTable dt)
        => dt.ToJson();
    #endregion

    #region async
    /// <summary>
    /// get aDataTable from Stream Data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="separator"></param>
    internal static Task<DataTable> FromStreamAsync(Stream data, char separator)
    {
        DataTable dt = new DataTable();
        return dt.FromStreamAsync(data, separator);
    }
    #endregion
}
