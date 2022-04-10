using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using netCorePlayground.Infra;
using netCorePlayground.Middlewares;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization((Action<Microsoft.AspNetCore.Authorization.AuthorizationOptions>) (config =>
{
    config.AddPolicy(Policies.UserPolicy, Policies.UserPolicyGenerator());

    config.AddPolicy(Policies.AdminRole, Policies.AdminRoleGenerator());
    config.AddPolicy(Policies.TesterUserRole, Policies.TesterUserRoleGenerator());
}));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Weather Forecast API", Version = "v1"});

    string actionSummaryXMLFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, actionSummaryXMLFile), true);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your access token in the text input below.\r\n\r\nExample: \"Bearer aBcDDE.MyToken\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionMiddleware>();
}

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1"); });

app.UseHttpsRedirection();

var corsAllowedOrigins = app.Configuration.GetSection("CorsAllowedOrigins").Get<List<string>>();
app.UseCors(options =>
{
    options.WithOrigins(corsAllowedOrigins.ToArray())
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();