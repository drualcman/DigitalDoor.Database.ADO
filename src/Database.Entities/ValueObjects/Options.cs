namespace Database.Entities.ValueObjects;
public record struct Options(LogOptions LogOptions = default, bool EnableSqlInjectionControl = true, bool EnableCharControl = true);
