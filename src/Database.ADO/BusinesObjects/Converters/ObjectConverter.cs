namespace Database.ADO.BusinesObjects.Converters;

internal static class ObjectConverter
{
    #region Methods
    #region TO
    internal static string ToJson(object o)
        => o.ToJson();
    #endregion
    #endregion

    #region async
    #region TO
    internal static async Task<string> ToJsonAsync(object o)
        => await o.ToJsonAsync();
    #endregion
    #endregion

}
