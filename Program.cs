using bagend_web_scraper.Config;
using bagend_web_scraper.Repository;
using bagend_web_scraper.StockMarket.Client;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.StockMarket.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbConfig>(
    builder.Configuration.GetSection("MongoDbConfig"));
builder.Services.Configure<PolygonApiConfig>(
    builder.Configuration.GetSection("PolygonApiConfig"));
builder.Services.Configure<EventApiConfig>(
    builder.Configuration.GetSection("EventApiConfig"));

// Add services to the container.

builder.Services.AddSingleton<DateProvider>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<TickerDataTargetEntityRepository>();
builder.Services.AddSingleton<TickerDataTargetService>();
builder.Services.AddSingleton<PolygonApiRESTClient>();
builder.Services.AddSingleton<EventApiRESTClient>();
builder.Services.AddSingleton<PolygonApiResponseProcessor>();
builder.Services.AddSingleton<OpenCloseStockDataScraper>();
builder.Services.AddSingleton<StockDataScraper>();
builder.Services.AddSingleton<IHostedService, StockDataScrapingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
