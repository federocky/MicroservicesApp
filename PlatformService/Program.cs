using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddOpenApi();
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("InMem"));

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

    builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

    builder.Services.AddControllers();

}


var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    PrepDb.PrepPopulation(app);

    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}




