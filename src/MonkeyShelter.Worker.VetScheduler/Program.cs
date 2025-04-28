using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Infrastructure;
using MonkeyShelter.Worker.VetScheduler;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddHostedService<VetCheckBackgroundService>();
builder.Services.AddDbContext<DataContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IVetChecksRepository, VetChecksRepository>();

var host = builder.Build();

host.Run();