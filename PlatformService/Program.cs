using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
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
    app.Run();
}




