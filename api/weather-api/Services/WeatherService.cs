using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using WeatherApi.Models;

namespace WeatherApi.Services;

public interface IWeatherService
{
    Task<WeatherSummary?> GetCurrentAsync(string city);
    Task<ForecastSummary?> GetForecastAsync(string city);
}

public class WeatherService(
    HttpClient httpClient,
    IMemoryCache cache,
    IConfiguration config) : IWeatherService
{
    private readonly string _apiKey = config["OpenWeatherMap:ApiKey"]!;
    private readonly string _units = config["OpenWeatherMap:Units"] ?? "metric";
    private readonly int _weatherCacheMins = int.Parse(config["Cache:WeatherDurationMinutes"] ?? "10");
    private readonly int _forecastCacheMins = int.Parse(config["Cache:ForecastDurationMinutes"] ?? "30");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<WeatherSummary?> GetCurrentAsync(string city)
    {
        var cacheKey = $"weather:{city.ToLower()}";

        if (cache.TryGetValue(cacheKey, out WeatherSummary? cached))
            return cached;

        var url = $"weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units={_units}";
        var response = await httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<WeatherResponse>(json, JsonOptions);

        if (data is null) return null;

        var summary = MapToSummary(data);
        cache.Set(cacheKey, summary, TimeSpan.FromMinutes(_weatherCacheMins));
        return summary;
    }

    public async Task<ForecastSummary?> GetForecastAsync(string city)
    {
        var cacheKey = $"forecast:{city.ToLower()}";

        if (cache.TryGetValue(cacheKey, out ForecastSummary? cached))
            return cached;

        var url = $"forecast?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units={_units}";
        var response = await httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var cityName = doc.RootElement.GetProperty("city").GetProperty("name").GetString() ?? city;
        var country = doc.RootElement.GetProperty("city").GetProperty("country").GetString() ?? "";

        var items = doc.RootElement.GetProperty("list").EnumerateArray().Select(item =>
        {
            var main = item.GetProperty("main");
            var weather = item.GetProperty("weather")[0];
            var wind = item.GetProperty("wind");

            return new ForecastItem(
                DateTime: DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).UtcDateTime,
                Temperature: main.GetProperty("temp").GetDouble(),
                Condition: weather.GetProperty("main").GetString() ?? "",
                Description: weather.GetProperty("description").GetString() ?? "",
                Icon: weather.GetProperty("icon").GetString() ?? "",
                WindSpeed: wind.GetProperty("speed").GetDouble(),
                Humidity: main.GetProperty("humidity").GetInt32()
            );
        });

        var summary = new ForecastSummary(cityName, country, items);
        cache.Set(cacheKey, summary, TimeSpan.FromMinutes(_forecastCacheMins));
        return summary;
    }

    private static WeatherSummary MapToSummary(WeatherResponse data) => new(
        City: data.Name,
        Country: data.Sys.Country,
        Temperature: data.Main.Temp,
        FeelsLike: data.Main.FeelsLike,
        TempMin: data.Main.TempMin,
        TempMax: data.Main.TempMax,
        Humidity: data.Main.Humidity,
        Condition: data.Weather[0].Main,
        Description: data.Weather[0].Description,
        Icon: data.Weather[0].Icon,
        WindSpeed: data.Wind.Speed,
        Visibility: data.Visibility,
        Sunrise: DateTimeOffset.FromUnixTimeSeconds(data.Sys.Sunrise).UtcDateTime,
        Sunset: DateTimeOffset.FromUnixTimeSeconds(data.Sys.Sunset).UtcDateTime,
        FetchedAt: DateTime.UtcNow
    );
}
