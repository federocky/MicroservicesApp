using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    builder.Services.AddHostedService<MessageBusSubscriber>();
    
    builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));

    builder.Services.AddScoped<ICommandRepo, CommandRepo>();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();    
    app.MapControllers();

    PrepDb.PrepPopulation(app);
    
    app.Run();
}

