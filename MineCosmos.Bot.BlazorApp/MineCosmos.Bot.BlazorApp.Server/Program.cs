using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MineCosmos.Bot.BlazorApp.Shared.Data;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Extensions;
using MineCosmos.Bot.Service;
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

//机器人相关服务
builder.Host.UseBotConfigureServices();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey( Encoding.ASCII.GetBytes(AppSettings.app(new string[] { "Jwt", "Secret" }) ?? "xxxxxxxxxxxxxx")),
        ValidateIssuer = true,
        ValidIssuer = "Issuer",
        ValidateAudience = false
    };
});
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddSingleton<IJwtService, JwtService>();

//测试



builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBootstrapBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

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

//授权鉴权
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#region 测试代码

#endregion


app.Run();

