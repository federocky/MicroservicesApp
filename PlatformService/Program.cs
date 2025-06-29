using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddOpenApi();

    var env = builder.Environment;
    if (env.IsProduction())
    {
        Console.WriteLine("--> Using SqlServer Db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
    }
    else
    {
        Console.WriteLine("--> Using InMem Db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("InMem"));    
    }    


    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

    builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

    builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

    builder.Services.AddGrpc();

    builder.Services.AddControllers();

}


var app = builder.Build();
{
    var isProduction = app.Environment.IsProduction();

    if (isProduction)
    {
        app.MapOpenApi();
    }

    PrepDb.PrepPopulation(app, isProduction);

    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapGrpcService<GrpcPlatformService>();

    app.MapGet("/protos/platforms.proto", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
    });

    app.Run();
}




