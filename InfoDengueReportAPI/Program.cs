using Microsoft.EntityFrameworkCore;
using InfoDengueReportAPI.Data;
using Serilog;
using InfoDengueReportAPI.Services;
using InfoDengueReportAPI.Services.Utils;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();



try
{
    Log.Information("Iniciando a aplicação...");
    builder.Services.AddScoped<IEpidemiologicalDataService, EpidemiologicalDataService>();
    builder.Services.AddScoped<IReportProcessManagerService, ReportProcessManagerService>();
    builder.Services.AddScoped<IReportService, ReportService>();
    builder.Services.AddScoped<IApiClient, ApiClient>();
    builder.Services.AddHttpClient<IApiClient, ApiClient>();
    builder.Services.AddDbContext<InfoDengueContext>(options =>
        options.UseSqlite("Data Source=infodengue.db"));

    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Aplicação iniciada com sucesso.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}
