using Microsoft.EntityFrameworkCore;
using Npgsql;
using RecommendationService.Application.Interfaces;
using RecommendationService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRecommendationDbContext, RecommendationDbContext>();
builder.Services.AddDbContext<RecommendationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString).Build();
    options.UseNpgsql(dataSourceBuilder);
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();
app.Run();