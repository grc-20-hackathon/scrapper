using scrapper.GraphQL;
using scrapper.Services;
using Neo4j.Driver;
using scrapper.GraphQL;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<scrapper.GraphQL.Query>();
    //.AddMutationType<Mutation>();

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
builder.Services.AddScoped<IJobParserService, JobParserService>();
builder.Services.AddHostedService<JobParserBackgroundService>();
var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "default_password"; 
builder.Services.AddSingleton<IDriver>(GraphDatabase.Driver("bolt://neo4j.db:7687", AuthTokens.Basic("neo4j", neo4jPassword)));

builder.Services.AddScoped<INeo4jService, Neo4jService>();
builder.Services.AddScoped<IAIService, AIService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseGraphQLVoyager();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowLocalhost");

app.MapGraphQL();

app.Run();











