using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZedCrestTest.Persistence.DBContexts;
using System;

namespace ZedCrestTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.Development.json", optional: true)

                        .Build();

            var logPath = config.GetValue<string>("Logger:LogPath");
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.LifeTime", LogEventLevel.Information)
                        .WriteTo.File(new JsonFormatter(), path: $@"{logPath}\{DateTime.Now:yyyy-MM-dd}\UserLogs.json",
                        rollingInterval: RollingInterval.Hour)
                        .CreateLogger();
            try
            {
                Log.Information("Start");
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ZedCrestContext>();
                        context.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal("An Error Occured during Migration", ex.StackTrace);
                    }
                }
                host.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal("An Error Occured, Application start-up failed", ex.StackTrace);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
