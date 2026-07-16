using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RecommendationService.API.Middlewares;
using RecommendationService.Application.Consumers.BranchConsumers;
using RecommendationService.Application.Consumers.CompanyConsumers;
using RecommendationService.Application.Consumers.CompanyServiceConsumers;
using RecommendationService.Application.Consumers.ComplaintEventConsumers;
using RecommendationService.Application.Consumers.EmployeeConsumers;
using RecommendationService.Application.Consumers.QueueEventConsumers;
using RecommendationService.Application.Consumers.ReviewEventConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.Application.Services;
using RecommendationService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5009, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });

    options.ListenAnyIP(5010, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});

builder.Services.AddMagicOnion();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BranchCreatedEventConsumer>();
    x.AddConsumer<BranchDeletedEventConsumer>();
    x.AddConsumer<CompanyCreatedEventConsumer>();
    x.AddConsumer<CompanyUpdatedEventConsumer>();
    x.AddConsumer<CompanyDeletedEventConsumer>();
    x.AddConsumer<CompanyServiceCreatedEventConsumer>();
    x.AddConsumer<CompanyServiceDeletedEventConsumer>();
    x.AddConsumer<EmployeeCreatedEventConsumer>();
    x.AddConsumer<EmployeeDeletedEventConsumer>();
    x.AddConsumer<QueueCompletedEventConsumer>();
    x.AddConsumer<QueueCancelledEventConsumer>();
    x.AddConsumer<ComplaintCreatedEventConsumer>();
    x.AddConsumer<ReviewCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetService<IConfiguration>();

        var host = configuration?["RabbitMQ:Host"] ?? "localhost";
        var port = configuration?.GetValue<ushort?>("RabbitMQ:Port") ?? 5672;
        var username = configuration?["RabbitMQ:Username"] ?? "guest";
        var password = configuration?["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, port, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddScoped<IRecommendationDbContext, RecommendationDbContext>();
builder.Services.AddSingleton<IRecommendationScoreService, RecommendationScoreService>();
builder.Services.AddDbContext<RecommendationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString).Build();
    options.UseNpgsql(dataSourceBuilder);
});


var app = builder.Build();
app.MapMagicOnionService<RecommendationService.Application.Services.RecommendationService>();


if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();


app.MapControllers();
app.Run();