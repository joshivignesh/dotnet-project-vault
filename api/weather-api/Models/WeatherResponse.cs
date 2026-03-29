namespace WeatherApi.Models;

public record WeatherResponse(
    string Name,
    MainData Main,
    WeatherData[] Weather,
    WindData Wind,
    SysData Sys,
    int Visibility,
    long Dt
);

public record MainData(
    double Temp,
    double FeelsLike,
    double TempMin,
    double TempMax,
    int Humidity,
    int Pressure
);

public record WeatherData(
    int Id,
    string Main,
    string Description,
    string Icon
);

public record WindData(double Speed, int Deg);

public record SysData(string Country, long Sunrise, long Sunset);
