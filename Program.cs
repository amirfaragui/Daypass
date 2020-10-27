using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace ValueCards
{
  public class Program
  {
    public static int Main(string[] args)
    {
      var path = AppDomain.CurrentDomain.BaseDirectory;

      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

      var logFileTemplate = Path.Combine(path, "Logs", "log-{Date}.txt");
      var outputTemplate = @"===> [{Timestamp:HH:mm:ss} {Level} {SourceContext}] {Message:lj}{NewLine}{Exception}";
      var logConfiguration = new LoggerConfiguration().ReadFrom.Configuration(configuration);
      logConfiguration.WriteTo.File(logFileTemplate, outputTemplate: outputTemplate);
      Log.Logger = logConfiguration.CreateLogger();

      try
      {
        Log.Information("Starting web host");
        CreateHostBuilder(args).Build().Run();
        return 0;
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Host terminated unexpectedly");
        return 1;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
              webBuilder.UseSerilog();
            })
            .UseWindowsService();
  }
}
