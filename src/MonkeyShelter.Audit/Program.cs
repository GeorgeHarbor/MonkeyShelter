using MassTransit;

using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Audit;

using MonkeyShelter.Application.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuditDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("AuditDatabase")));

builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigins",
        policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()));

builder.Services.AddMassTransit(x => x.UsingRabbitMq((context, cfg) =>
    {
        var rmq = builder.Configuration.GetSection("RabbitMq");
        cfg.Host(rmq["Host"], h =>
        {
            h.Username(rmq["Username"]!);
            h.Password(rmq["Password"]!);
        });

        cfg.ReceiveEndpoint("main-queue", e =>
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

               e.Bind<VetCheckScheduled>();
               e.Handler<VetCheckScheduled>(ctx => HandleAuditEvent(ctx));

               e.Bind<MonkeyArrived>();
               e.Handler<MonkeyArrived>(ctx => HandleAuditEvent(ctx));

               e.Bind<MonkeyDeparted>();
               e.Handler<MonkeyDeparted>(ctx => HandleAuditEvent(ctx));

               e.Bind<MonkeyWeightChanged>();
               e.Handler<MonkeyWeightChanged>(ctx => HandleAuditEvent(ctx));

               e.Bind<ManagerShelterAssigned>();
               e.Handler<ManagerShelterAssigned>(ctx => HandleAuditEvent(ctx));

               e.Bind<ReportGenerated>();
               e.Handler<ReportGenerated>(ctx => HandleAuditEvent(ctx));

               e.Bind<ShelterCreated>();
               e.Handler<ShelterCreated>(ctx => HandleAuditEvent(ctx));
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

app.MapGet("", async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
        return await db.Set<AuditLog>().ToListAsync();
    }
    ;
});
app.UseCors("AllowSpecificOrigins");
app.Run();