using System.Text;
using MineCosmos.Bot.BlazorApp.Shared.Data;
using MineCosmos.Bot.Extensions;
using MineCosmos.Bot.Service.Bot;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder
          //.SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            //.AllowCredentials()
            );
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBootstrapBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
//机器人相关服务
builder.Host.UseBotConfigureServices();
// 增加 Table 数据服务操作类
builder.Services.AddTableDemoDataService();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseCors("AllowAnyOrigin");

app.UseStaticFiles();


app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
