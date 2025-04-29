using StockWatchlist.Services;
using StockWatchlist.Repositories;

var builder = WebApplication.CreateBuilder(args);

var twelveDataBaseUrl = builder.Configuration["TwelveData:BaseUrl"]
                         ?? throw new Exception("Twelve Data Base URL is not set in the configuration.");
var twelveDataApiKey = builder.Configuration["TwelveData:ApiKey"]
                        ?? throw new Exception("Twelve Data API Key is not set in the configuration.");

builder.Services.AddSingleton(twelveDataBaseUrl);
builder.Services.AddSingleton(twelveDataApiKey);
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddHttpClient<IStockService, StockService>(client =>
{
    client.BaseAddress = new Uri(twelveDataBaseUrl);
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StockWatchlist API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
