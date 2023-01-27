namespace Database.ADO.BusinesObjects.Entities.Models;

internal class Columns
{
    public string TableShortName { get; set; }
    public int TableIndex { get; set; }
    public string ColumnName { get; set; }
    public string PropertyType { get; set; }
    public PropertyInfo Column { get; set; }
    public DatabaseAttribute Options { get; set; }
}
