﻿namespace Database.ADO.BusinesObjects.Extensions;

internal static class DataTableExtension
{
    #region columns
    #region methods

    /// <summary>
    /// Get all column names from the table send
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static List<string> ColumnNamesToList(this DataTable dt)
    {
        List<string> names = new List<string>();

        foreach (DataColumn item in dt.Columns)
        {
            names.Add(item.ColumnName.ToLower());
        }

        return names;
    }

    /// <summary>
    /// Get all column names from the table send
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static string[] ColumnNamesToArray(this DataTable dt)
    {
        List<string> names = new List<string>();

        foreach (DataColumn item in dt.Columns)
        {
            names.Add(item.ColumnName.ToLower());
        }

        return names.ToArray();
    }

    #endregion

    #region async
    /// <summary>
    /// Get all column names from the table send
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static Task<List<string>> ColumnNamesToListAsync(this DataTable dt) => Task.FromResult(dt.ColumnNamesToList());

    /// <summary>
    /// Get all column names from the table send
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static Task<string[]> ColumnNamesToArrayAsync(this DataTable dt) => Task.FromResult(dt.ColumnNamesToArray());

    #endregion
    #endregion

    #region methods
    #region TO
    /// <summary>
    /// Convert DataTable to Json
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static string ToJson(this DataTable dt)
    {
        if (dt is not null)
        {
            //https://stackoverflow.com/questions/17398019/convert-datatable-to-json-in-c-sharp                
            StringBuilder jsonString = new StringBuilder();
            int r = dt.Rows.Count;
            int c = dt.Columns.Count;
            if (r > 0)
            {
                jsonString.Append("[");
                for (int row = 0; row < r; row++)
                {
                    jsonString.Append("{");
                    for (int col = 0; col < c; col++)
                    {
                        jsonString.Append("\"");
                        jsonString.Append(dt.Columns[col].ColumnName);
                        jsonString.Append("\":");

                        bool last = !(col < c - 1);
                        jsonString.Append(GenerateJsonProperty(dt, row, col, last));
                    }
                    jsonString.Append(row == dt.Rows.Count - 1 ? "}" : "},");
                }
                jsonString.Append("]");
                return jsonString.ToString();
            }
        }
        return string.Empty;
    }

    #endregion

    #region FROM
    /// <summary>
    /// Parse DataTable from Stream Data
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="data"></param>
    /// <param name="separator"></param>
    internal static DataTable FromStream(this DataTable dt, Stream data, char separator)
    {
        using StreamReader sr = new StreamReader(data);
        string[] headers = sr.ReadLine().Split(separator);
        foreach (string header in headers)
        {
            dt.Columns.Add(header);
        }
        while (!sr.EndOfStream)
        {
            string[] rows = sr.ReadLine().Split(separator);
            DataRow dr = dt.NewRow();
            for (int i = 0; i < headers.Length; i++)
            {
                dr[i] = rows[i];
            }
            dt.Rows.Add(dr);
        }
        return dt;
    }

    /// <summary>
    /// Convert from csv file into a DataTable
    /// </summary>
    /// <param name="filePath">full path to get the file to part into a datatable</param>
    /// <param name="separator"></param>
    /// <returns></returns>
    internal static DataTable FromFile(this DataTable dt, string filePath, char separator)
    {
        using StreamReader sr = new StreamReader(filePath);
        return dt.FromStream(sr.BaseStream, separator);
    }

    /// <summary>
    /// Parse JsonString to datatable
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    internal static DataTable FromJson(this DataTable dt, string jsonString)
    {
        //http://www.c-sharpcorner.com/blogs/convert-json-string-to-datatable-in-asp-net1
        if (!string.IsNullOrEmpty(jsonString) && jsonString.ToLower() != "undefined")
        {
            jsonString = jsonString.Replace("}, {", "},{");
            jsonString = CheckComa(jsonString);
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "").Trim();
                        if (!ColumnsName.Contains(ColumnsNameString))
                            ColumnsName.Add(ColumnsNameString);
                        else
                        {
                            //if found more than one column with same name add the id to difference the column name
                            ColumnsName.Add(ColumnsNameString + (ColumnsName.Count() - 1).ToString());
                        }
                    }
                    catch
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                int columnNumber = 0;       //reset index of the column per each element
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "").Trim();
                        nr[columnNumber] = RowDataString;       //because the columns always come in same order use the index not the name
                        columnNumber++;
                    }
                    catch
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
        }
        return dt;
    }
    #endregion
    #endregion

    #region async
    #region TO
    /// <summary>
    /// Convertir uyn DataTable en un objero JSON
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    internal static Task<string> ToJsonAsync(this DataTable dt)
        => Task.FromResult(dt.ToJson());
    #endregion

    #region FROM

    /// <summary>
    /// Parse DataTable from Stream Data
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="data"></param>
    /// <param name="separator"></param>
    internal static async Task<DataTable> FromStreamAsync(this DataTable dt, Stream data, char separator)
    {
        StreamReader sr = new StreamReader(data);
        string head = await sr.ReadLineAsync();
        string[] headers = head.Split(separator);
        foreach (string header in headers)
        {
            dt.Columns.Add(header);
        }
        while (!sr.EndOfStream)
        {
            string rowHead = await sr.ReadLineAsync();
            string[] rows = rowHead.Split(separator);
            DataRow dr = dt.NewRow();
            for (int i = 0; i < headers.Length; i++)
            {
                dr[i] = rows[i];
            }
            dt.Rows.Add(dr);
        }
        return dt;
    }

    /// <summary>
    /// Convert from csv file into a DataTable
    /// </summary>
    /// <param name="filePath">full path to get the file to part into a datatable</param>
    /// <param name="separator"></param>
    /// <returns></returns>
    internal static async Task<DataTable> FromFileAsync(this DataTable dt, string filePath, char separator)
    {
        StreamReader sr = new StreamReader(filePath);
        return await dt.FromStreamAsync(sr.BaseStream, separator);
    }

    /// <summary>
    /// Parse JsonString to datatable
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    internal static Task<DataTable> FromJsonAsync(this DataTable dt, string jsonString)
        => Task.FromResult(dt.FromJson(jsonString));
    #endregion
    #endregion

    #region Helpers   
    private static string GenerateJsonProperty(DataTable dt, int row, int col, bool isLast = false)
    {
        // IF LAST PROPERTY THEN REMOVE 'COMMA'  IF NOT LAST PROPERTY THEN ADD 'COMMA'
        string addComma = isLast ? "" : ",";
        StringBuilder jsonString = new StringBuilder();
        if (dt.Rows[row][col] == DBNull.Value)
            jsonString.Append(" null ");
        else if (dt.Columns[col].DataType == typeof(DateTime))
        {
            jsonString.Append("\"");
            jsonString.Append(((DateTime)dt.Rows[row][col]).ToString("O"));
            jsonString.Append("\"");
        }
        else if (dt.Columns[col].DataType == typeof(string))
        {
            jsonString.Append("\"");
            jsonString.Append(dt.Rows[row][col].ToString().Replace("\"", "\\\""));
            jsonString.Append("\"");
        }
        else if (dt.Columns[col].DataType == typeof(bool))
        {
            jsonString.Append(Convert.ToBoolean(dt.Rows[row][col]) ? "true" : "false");
        }
        else if (dt.Columns[col].DataType == typeof(short) ||
            dt.Columns[col].DataType == typeof(int) ||
            dt.Columns[col].DataType == typeof(long) ||
            dt.Columns[col].DataType == typeof(double) ||
            dt.Columns[col].DataType == typeof(decimal) ||
            dt.Columns[col].DataType == typeof(float) ||
            dt.Columns[col].DataType == typeof(byte) ||
            dt.Columns[col].DataType == typeof(int) ||
            dt.Columns[col].DataType == typeof(float) ||
            dt.Columns[col].DataType == typeof(long) ||
            dt.Columns[col].DataType == typeof(short))
        {
            try
            {
                jsonString.Append($"{dt.Rows[row][col]}");
            }
            catch
            {
                jsonString.Append("null");
            }
        }
        else
        {
            jsonString.Append("\"");
            jsonString.Append(dt.Rows[row][col].ToString().Replace("\"", "\\\""));
            jsonString.Append("\"");
        }
        jsonString.Append(addComma);
        return jsonString.ToString();
    }

    /// <summary>
    /// Check don't have a , between " on the text.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string CheckComa(string text)
    {
        string retorno = string.Empty;
        string caracter;
        string siguiente;
        for (int i = 0; i < text.Length; i++)
        {
            caracter = text.Substring(i, 1);
            int n = i + 1;
            if (n < text.Length)
            {
                siguiente = text.Substring(n, 1);
                if (caracter == "," && siguiente != "\"")
                {
                    if (caracter == "," && siguiente != "{") retorno += string.Empty;
                    else if (caracter == "," && siguiente == " ")
                    {
                        n++;
                        siguiente = text.Substring(n, 1);
                        if (caracter == "," && siguiente != "\"")
                        {
                            if (caracter == "," && siguiente != "{") retorno += string.Empty;
                            else retorno += caracter;
                        }
                        else retorno += caracter;
                    }
                    else retorno += caracter;
                }
                else retorno += caracter;
            }
            else retorno += caracter;
        }
        return retorno;
    }

    #endregion
}
