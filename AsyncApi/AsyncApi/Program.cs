var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () =>
{
    Thread.Sleep(1000);
    return Random.Shared.Next(100);
});

app.Run();