namespace Database.Entities.ValueObjects;
public record struct LogOptions(bool LogResults = false, string LogFolder = "logs");
