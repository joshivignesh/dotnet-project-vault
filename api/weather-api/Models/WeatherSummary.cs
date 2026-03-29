namespace WeatherApi.Models;

public record WeatherSummary(
    string City,
    string Country,
    double Temperature,
    double FeelsLike,
    double TempMin,
    double TempMax,
    int Humidity,
    string Condition,
    string Description,
    string Icon,
    double WindSpeed,
    int Visibility,
    DateTime Sunrise,
    DateTime Sunset,
    DateTime FetchedAt
);

public record ForecastSummary(
    string City,
    string Country,
    IEnumerable<ForecastItem> Items
);

public record ForecastItem(
    DateTime DateTime,
    double Temperature,
    string Condition,
    string Description,
    string Icon,
    double WindSpeed,
    int Humidity
);
