namespace Database.ADO.BusinesObjects.Entities.Models;

internal class ColumnToObjectResponse
{
    public object InUse { get; set; }
    public string ActualTable { get; set; }
    public bool IsList { get; set; }
    public object PropertyListData { get; set; }
    public string PropertyListName { get; set; }
    public object PropertyListInstance { get; set; }

}
