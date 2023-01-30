namespace Database.Entities.ValueObjects;

public sealed record TableName(string Name, string ShortName,
    string ShortReference, InnerDirection Inner, string Column,
    string InnerIndex, string ClassName, PropertyInfo Instance = null);
