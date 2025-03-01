using Biz.WebAPI.DataProviders;
using Biz.WebAPI.DBContext;
using Biz.WebAPI.Services;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddScoped<DemoService, DemoService>();

// Add DataProvider
builder.Services.AddScoped<OracleDbContext,OracleDbContext>();
builder.Services.AddScoped<DemoDataProvider, DemoDataProvider>();
builder.Services.AddScoped<HandSideDBProvider, HandSideDBProvider>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// nlog
builder.Logging.ClearProviders();
builder.Logging.AddNLog("Config/NLog.config");
builder.Host.UseNLog();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
