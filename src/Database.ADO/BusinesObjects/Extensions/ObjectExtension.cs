namespace Database.ADO.BusinesObjects.Extensions;

internal static class ObjectExtension
{
    #region converters
    #region Methods
    #region TO
    internal static string ToJson(this object o)
    {
        string data;
        try
        {
            if (o != null)
            {
                if (o.GetType() == typeof(DataSet))
                    data = DataSetConverter.ToJson((DataSet)o);
                else if (o.GetType() == typeof(DataTable))
                {
                    data = DataTableConverter.ToJson((DataTable)o);
                }
                else if (o.GetType() == typeof(DataView))
                {
                    DataView dv = (DataView)o;
                    DataTable dt = dv.ToTable();
                    data = dt.ToJson();
                }
                else data = JsonSerializer.Serialize(o);
            }
            else data = "{\"Object\":\"NULL\"}";
        }
        catch (Exception ex)
        {
            data = $"{{\"Exception\":\"{ex.Message}\"}}";
        }
        return data;
    }
    #endregion
    #endregion

    #region async
    #region TO
    internal static Task<string> ToJsonAsync(this object o)
        => Task.FromResult(o.ToJson());
    #endregion
    #endregion
    #endregion

    /// <summary>
    /// Get a value from a porperty on the object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static object GetPropValue(this object obj, string name)
    {
        if (obj == null) return null;
        else
        {
            // Split property name to parts (propertyName could be hierarchical, like obj.subobj.subobj.property
            string[] propertyNameParts = name.Split('.');

            foreach (string part in propertyNameParts)
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) return null;
                else obj = info.GetValue(obj, null);
            }
            return obj;
        }
    }

    /// <summary>
    /// Get a value from the object on the specific type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static T GetPropValue<T>(this object obj, string name)
    {
        // throws InvalidCastException if types are incompatible
        object retval = obj.GetPropValue(name);
        if (retval == null) return default;
        else return (T)retval;
    }
}
