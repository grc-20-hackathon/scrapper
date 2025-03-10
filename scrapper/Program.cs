using scrapper.GraphQL;
using scrapper.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using scrapper.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<scrapper.GraphQL.Query>()
    .AddMutationType<Mutation>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder.WithOrigins("http://localhost")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationsAssembly));
});
builder.Services.AddScoped<IJobParserService, JobParserService>();
builder.Services.AddHostedService<JobParserBackgroundService>();
builder.Services.AddScoped<IAIService, AIService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseGraphQLVoyager();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowLocalhost");

app.MapGraphQL();

app.Run();











