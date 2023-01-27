namespace Database.ADO.Entities.Helpers;

public static class Utils
{
    public static string GetCatalog(string queryString)
    {
        string[] ruta = queryString.Split(';');
        string catalog = string.Empty;
        foreach (string Categoria in ruta)
        {
            string[] desglose = Categoria.Split('=');
            if (desglose[0].ToLower().IndexOf("catalog") >= 0) catalog = desglose[1];
        }
        return catalog;
    }

    public static string GetServer(string queryString)
    {
        string[] ruta = queryString.Split(';');
        string Source = string.Empty;
        foreach (string Categoria in ruta)
        {
            string[] desglose = Categoria.Split('=');
            if (desglose[0].ToLower().IndexOf("source") >= 0) Source = desglose[1];
        }
        return Source;
    }

}