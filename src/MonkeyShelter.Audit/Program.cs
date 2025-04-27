using MassTransit;

using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Audit;

using MonkeyShelter.Application.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuditDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("AuditDatabase")));

builder.Services.AddMassTransit(x => x.UsingRabbitMq((context, cfg) =>
    {
        var rmq = builder.Configuration.GetSection("RabbitMq");
        cfg.Host(rmq["Host"], h =>
        {
            h.Username(rmq["Username"]!);
            h.Password(rmq["Password"]!);
        });

        cfg.ReceiveEndpoint("audit-queue", e =>
           {
               e.ConfigureConsumeTopology = false;
               async Task HandleAuditEvent<T>(ConsumeContext<T> ctx) where T : class
               {
                   using var scope = context.GetRequiredService<IServiceScopeFactory>().CreateScope();
                   var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
                   var payload = JsonSerializer.Serialize(ctx.Message);

                   db.AuditLogs.Add(new AuditLog
                   {
                       EventType = typeof(T).Name,
                       Payload = payload,
                       ReceivedAt = DateTime.UtcNow
                   });

                   await db.SaveChangesAsync();
               }

               e.Bind<UserRegistered>();
               e.Handler<UserRegistered>(ctx => HandleAuditEvent(ctx));

           });

    }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    db.Database.Migrate();
}

app.Run();