﻿namespace Database.ADO.BusinesObjects.Helpers;

internal sealed class ObjectHelpers
{
    public static bool IsGenericList(string o)
    {
        return o.Contains("System.Collections.Generic.List");
    }

    public static bool IsGenericList(object o)
    {
        Type oType = o.GetType();
        return oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>);
    }

    public static bool IsGenericList<T>(object o)
    {
        Type oType = o.GetType();
        return oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<T>);
    }
}
