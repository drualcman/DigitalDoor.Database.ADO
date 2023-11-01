namespace Database.ADO.Entities.Helpers;

public class SqlQueryTranslator
{
    private List<TableName> tableNamesBK = new List<TableName>();
    public IEnumerable<TableName> TableNames => tableNamesBK;

    readonly Dictionary<string, object> WhereRequired;

    public SqlQueryTranslator(Dictionary<string, object> whereRequired)
    {
        WhereRequired = whereRequired;
    }

    /// <summary>
    /// Get the select from the properties name about the model send
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    public string SetQuery<TModel>()
    {
        PropertyInfo[] properties = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        TableNamesHelper tableNames = new TableNamesHelper();
        tableNames.AddTableNames<TModel>();
        tableNamesBK = new List<TableName>(tableNames.TableNames);
        string shortName = tableNamesBK[0].ShortName;

        StringBuilder retorno = new StringBuilder("SELECT ");
        int c = properties.Length;
        for (int i = 0; i < c; i++)
        {
            DatabaseAttribute field = properties[i].GetCustomAttribute<DatabaseAttribute>();
            string fieldName;
            if (field is not null)
            {
                if (!field.Ignore)
                {
                    if (field.Inner != InnerDirection.NONE)
                    {
                        //add columns from the property model
                        InnerColumns(properties[i], retorno, field, shortName);
                        fieldName = string.Empty;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(field.Name))
                            fieldName = properties[i].Name;
                        else
                            fieldName = field.Name;
                    }
                }
                else
                    fieldName = string.Empty;
            }
            else
                fieldName = properties[i].Name;
            if (!string.IsNullOrEmpty(fieldName))
                retorno.Append($" {tableNamesBK[0].ShortName}.[{fieldName}] [{tableNamesBK[0].ShortName}.{fieldName}],");

        }
        retorno.Remove(retorno.Length - 1, 1);
        retorno.Append(Environment.NewLine);
        retorno.Append($"FROM [{tableNamesBK[0].Name}] {tableNamesBK[0].ShortName} ");
        if (tableNamesBK.Count() > 1)
        {
            //add inner joins depending of the model database attributes
            int tc = tableNamesBK.Count();
            for (int i = 1; i < tc; i++)
            {
                retorno.Append(Environment.NewLine);
                retorno.Append($"\t{tableNamesBK[i].Inner} JOIN [{tableNamesBK[i].Name}] {tableNamesBK[i].ShortName} on {tableNamesBK[i].ShortReference}.{(string.IsNullOrEmpty(tableNamesBK[i].InnerIndex) ? string.IsNullOrEmpty(tableNamesBK[i].Column) ? $"{tableNamesBK[i].Name}Id" : tableNamesBK[i].Column : tableNamesBK[i].InnerIndex)} = {tableNamesBK[i].ShortName}.{(string.IsNullOrEmpty(tableNamesBK[i].Column) ? $"{tableNamesBK[i].Name}Id" : tableNamesBK[i].Column)}");
            }
        }

        retorno.Append(Environment.NewLine);
        if (WhereRequired is not null)
        {
            bool foundSome = false;
            retorno.Append($"WHERE ");
            for (int i = 0; i < c; i++)
            {
                DatabaseAttribute field = properties[i].GetCustomAttribute<DatabaseAttribute>();
                string fieldName = properties[i].Name;
                if (field is not null)
                {
                    if (!string.IsNullOrEmpty(field.Name)) fieldName = field.Name;
                    if (field.IndexKey)
                    {
                        if (string.IsNullOrEmpty(field.IndexedName) && WhereRequired.ContainsKey(fieldName))
                        {
                            foundSome = true;
                            retorno.Append($" {tableNamesBK[0].ShortName}.[{fieldName}] = {GetWhereValue(fieldName)} ");
                            retorno.Append("AND");
                        }
                        else
                        {
                            if (WhereRequired.ContainsKey(field.IndexedName))
                            {
                                foundSome = true;
                                retorno.Append($" {tableNamesBK[0].ShortName}.[{fieldName}] = {GetWhereValue(field.IndexedName)} ");
                                retorno.Append("AND");
                            }
                        }
                    }
                }
                else
                {
                    if (WhereRequired.ContainsKey(fieldName))
                    {
                        foundSome = true;
                        retorno.Append($" {tableNamesBK[0].ShortName}.[{fieldName}] = {GetWhereValue(fieldName)} ");
                        retorno.Append("AND");
                    }
                }
            }
            if (foundSome) retorno.Remove(retorno.Length - 3, 3);
            else retorno.Remove(retorno.Length - 7, 7);
        }
        return retorno.ToString();
    }

    /// <summary>
    /// Get the select from the properties name about the model send
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    public string SetQuery<TModel>(string indexColumn, int startIndex, int numElements)
    {
        StringBuilder retorno = new StringBuilder(SetQuery<TModel>());
        if(!string.IsNullOrEmpty(indexColumn) && numElements > 0)
        {
            retorno.Append($" ORDER BY {indexColumn}");
            retorno.Append($" OFFSET {startIndex} ROWS");
            retorno.Append($" FETCH NEXT {numElements} ROWS ONLY ");
        }
        return retorno.ToString();
    }

    private void InnerColumns(PropertyInfo column, StringBuilder retorno,
        DatabaseAttribute origin, string shortReference)
    {
        Type t = column.PropertyType;
        PropertyInfo[] properties;
        DatabaseAttribute table;
        if (ObjectHelpers.IsGenericList(column.PropertyType.FullName))
        {
            properties = column.PropertyType.GetGenericArguments()[0].GetProperties();
            table = column.PropertyType.GetGenericArguments()[0].GetCustomAttribute<DatabaseAttribute>();
        }
        else
        {
            properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            table = t.GetCustomAttribute<DatabaseAttribute>();
            TableName father;
            if (table == null) father = tableNamesBK.Find(r => r.Name == t.Name);
            else father = tableNamesBK.Find(r => r.Name == table.Name);
            InnerColumns(properties, father?.ShortName, retorno);
        }
    }

    private void InnerColumns(PropertyInfo[] properties, string shortName, StringBuilder retorno)
    {
        int c = properties.Length;
        for (int i = 0; i < c; i++)
        {
            DatabaseAttribute field = properties[i].GetCustomAttribute<DatabaseAttribute>();
            string fieldName = properties[i].Name;
            if (field is not null)
            {
                if (!field.Ignore)
                {
                    if (field.Inner != InnerDirection.NONE)
                    {
                        //add columns from the property model
                        InnerColumns(properties[i], retorno, field, shortName);
                        fieldName = string.Empty;
                    }
                    else
                    {
                        if (ObjectHelpers.IsGenericList(properties[i].PropertyType.FullName))
                            fieldName = string.Empty;
                        else
                        {
                            if (string.IsNullOrEmpty(field.Name))
                                fieldName = properties[i].Name;
                            else
                                fieldName = field.Name;
                        }
                    }
                }
                else
                    fieldName = string.Empty;
            }
            if (!string.IsNullOrEmpty(fieldName))
                retorno.Append($" {shortName}.[{fieldName}] [{shortName}.{fieldName}],");
        }
    }

    private object GetWhereValue(string key)
    {
        return WhereRequired.Where(k => k.Key == key).FirstOrDefault().Value;
    }
}
