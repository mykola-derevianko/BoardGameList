using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuer 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"])
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
    options.AddPolicy(name: "AnyOrigin", cfg =>
    {
        cfg.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();


//Configure Swagger only if the configuration setting is true
var useSwagger = builder.Configuration.GetValue<bool>("UseSwagger");

if (useSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

//Minimal web API endpoints
app.MapGet("/", context =>
{
    context.Response.Redirect("/boardgames");
    return Task.CompletedTask;
});

app.MapGet("/error", [EnableCors("AnyOrigin")][ResponseCache(NoStore = true)] () => Results.Problem());

app.MapGet("/error/test", [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)] () =>
{
    throw new Exception("This is a test exception");
});

app.MapControllers();
app.Run();
