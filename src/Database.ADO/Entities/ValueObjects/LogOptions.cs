namespace Database.ADO.Entities.ValueObjects;
public record struct LogOptions(bool LogResults = false, string LogFolder = "");
