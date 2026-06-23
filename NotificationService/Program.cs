using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Email;
using NotificationService.Grpc.Services;
using NotificationService.Grpc.Contracts;
using NotificationService.Models;
using NotificationService.Repositories;
using Elastic.Clients.Elasticsearch;
using NotificationService.Configuration;
using ProtoBuf.Grpc.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddCodeFirstGrpc();

builder.Services.AddScoped<IRepository<EmailLetter>, EmailLetterRepository>();

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.AddScoped<INotificationGrpcService, NotificationGrpcService>();
builder.Services.AddSingleton(new ElasticsearchClient(

    new ElasticsearchClientSettings(new Uri("http://localhost:9200"))));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5111, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

app.MapGet("/", () => "NotificationService is running");
app.MapGrpcService<NotificationGrpcService>();

app.MapPost("/debug/send-otp", async (
    SendOtpEmailRequest request,
    INotificationGrpcService notificationService) =>
{
    var response = await notificationService.SendOtpEmailAsync(request);
    return Results.Ok(response);
});

app.Run();