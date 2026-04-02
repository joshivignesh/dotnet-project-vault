using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenWeatherMap:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

app.MapGet("/weather/{city}", async (string city, IWeatherService svc) =>
{
    var result = await svc.GetCurrentAsync(city);
    return result is null
        ? Results.NotFound(new { error = $"City '{city}' not found." })
        : Results.Ok(result);
})
.WithName("GetCurrentWeather")
.WithSummary("Get current weather for a city");

app.MapGet("/weather/{city}/forecast", async (string city, IWeatherService svc) =>
{
    var result = await svc.GetForecastAsync(city);
    return result is null
        ? Results.NotFound(new { error = $"City '{city}' not found." })
        : Results.Ok(result);
})
.WithName("GetForecast")
.WithSummary("Get 5-day forecast for a city");

app.Run();
