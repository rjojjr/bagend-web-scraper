using bagend_web_scraper.Config;
using bagend_web_scraper.Repository;
using bagend_web_scraper.StockMarket.Client;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.StockMarket.Operations;
using bagend_web_scraper.StockMarket.Service;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var policyName = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      builder =>
                      {
                          builder
                            .WithOrigins("http://localhost:3000") // specifying the allowed origin
                            .WithMethods("GET", "POST", "PATCH") // defining the allowed HTTP method
                            .AllowAnyHeader(); // allowing any header to be sent
                      });
});

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
builder.Services.AddSingleton<OperationProcessor, ThrottledFIFOOperationProcessor>();
builder.Services.AddSingleton<OpenCloseStockDataScraper>();
builder.Services.AddSingleton<StockDataScraper>();
builder.Services.AddSingleton<IHostedService, StockDataScrapingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "bagend Web Scraper API",
        Description = "A dotnet application to scrap ML training data from the web"
    });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "bagend-web-scraper.xml");
    options.IncludeXmlComments(filePath);
});

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
app.UseCors(policyName);
app.Run();
