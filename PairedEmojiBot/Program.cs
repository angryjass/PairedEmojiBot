using Microsoft.EntityFrameworkCore;
using PairedEmojiBot;
using PairedEmojiBot.Db;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

Console.WriteLine("Начало миграций на бд");
using (var context = new PairedEmojiBotContext())
{
    context.Database.Migrate();
}
Console.WriteLine("Миграций на бд применены");

Console.WriteLine("Запущен бот " + MainClass.bot.GetMeAsync().Result.FirstName);

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, // receive all update types
};
MainClass.bot.StartReceiving(
    MainClass.HandleUpdateAsync,
    MainClass.HandleErrorAsync,
    receiverOptions,
    cancellationToken
);

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}