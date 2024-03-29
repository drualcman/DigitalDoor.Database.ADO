﻿namespace Database.ADO.BusinesObjects.Extensions;

internal static class DataRowExtension
{
    #region methods
    /// <summary>
    /// Get all column names from the row send
    /// </summary>
    /// <param name="dr"></param>
    /// <returns></returns>
    internal static string[] ColumnNamesToArray(this DataRow dr)
    {
        List<string> names = new List<string>();

        foreach (DataColumn item in dr.Table.Columns)
        {
            names.Add(item.ColumnName);
        }

        return names.ToArray();
    }

    /// <summary>
    /// Get all column names from the table send
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static List<string> ColumnNamesToList(this DataRow dr)
    {
        List<string> names = new List<string>();

        foreach (DataColumn item in dr.ItemArray)
        {
            names.Add(item.ColumnName);
        }

        return names;
    }
    #endregion

    #region Async
    /// <summary>
    /// Get all column names from the row send
    /// </summary>
    /// <param name="dr"></param>
    /// <returns></returns>
    internal static Task<string[]> ColumnNamesToArrayAsync(this DataRow dr) => Task.FromResult(dr.ColumnNamesToArray());

    internal static Task<List<string>> ColumnNamesToListAsync(this DataRow dr) => Task.FromResult(dr.ColumnNamesToList());
    #endregion


}

