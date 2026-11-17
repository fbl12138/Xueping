using XuePing.Application;
using Serilog;
using System.Text.Encodings.Web;
using System.Text.Json;
using XuePing.Api.Middleware;
using XuePing.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using XuePing.Infrastructure.Data;
using System.Net.WebSockets;
using XuePing.Domain.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using XuePing.Domain.Common;
using Microsoft.OpenApi;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using XuePing.Api.HostedServices;


Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/xueping.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
// 本地私有配置：存在才加载，用来放真实连接串与密钥（该文件已被 .gitignore 排除）
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
builder.Host.UseSerilog();
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        //Redis登录态对应
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?.FindFirst("userId")?.Value;
                if(userIdClaim == null)
                {
                    context.Fail("token 缺少 userId");
                    return;
                }
                var userId = userIdClaim;
                var cache = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                var jwtOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtOptions>>().Value;
                var storedToken = await cache.GetStringAsync($"login:token:{userId}");
                var currentToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if(string.IsNullOrEmpty(storedToken) || storedToken != currentToken)
                {
                    context.Fail("token 无效");
                    return;
                }
                // 刷新token过期时间
                await cache.SetStringAsync($"login:token:{userId}", storedToken, TimeSpan.FromMinutes(jwtOptions.ExpireMinutes), context.HttpContext.RequestAborted);
            }
        };
    });
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "请输入JWT Token(不加Bearer 前缀)"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
builder.Services.AddApplication();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<SeckillOrderWorker>();
    

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

    app.Run();
