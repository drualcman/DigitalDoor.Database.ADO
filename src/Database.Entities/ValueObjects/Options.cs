namespace Database.Entities.ValueObjects;
public record struct Options(LogOptions LogOptions = default, bool EnableDatabaseControl = true, bool ChrControl = true);
