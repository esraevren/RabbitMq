using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqWeb.WatermarkApp.BackgroundServices;
using RabbitMqWeb.WatermarkApp.Models;
using RabbitMqWeb.WatermarkApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var rabbitMqConfig = builder.Configuration.GetConnectionString("RabbitMq");
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(rabbitMqConfig),
});

builder.Services.AddSingleton<RabbitMqClientService>();
builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddDbContext<AppDbContext>(optionsAction =>
{
    optionsAction.UseInMemoryDatabase("WatermarkAppDb");
});

builder.Services.AddHostedService<ImageWatermarkBackgroundService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
