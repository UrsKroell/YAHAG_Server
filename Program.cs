using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using YAHGA_Server.Database;
using YAHGA_Server.Managers;
using YAHGA_Server.Managers.Interfaces;
using YAHGA_Server.PacketHandlers;
using YAHGA_Server.PacketHandlers.Interfaces;

namespace YAHGA_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json"
                    , optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "YAHGA_")
                .AddEnvironmentVariables();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            Log.Logger.Information("Configuration Loaded");
            Log.Logger.Information("Starting Server");
            
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("default");
                    services.AddDbContext<DatabaseContext>(optionsBuilder =>
                    {
#if DEBUG
                        optionsBuilder.UseMySql(connectionString
                                , new MariaDbServerVersion(ServerVersion.AutoDetect(connectionString)), options =>
                                {
                                    options.EnableRetryOnFailure(15);
                                })
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging();
#else
                        optionsBuilder.UseMySql(connectionString
                                , new MariaDbServerVersion(ServerVersion.AutoDetect(connectionString)), options =>
                                {
                                    options.EnableRetryOnFailure(15);
                                })
#endif
                        
                    }, ServiceLifetime.Singleton);
                    
                    services.AddSingleton<IDatabaseManager, DatabaseManager>();
                    services.AddSingleton<IFilesystemManager, FilesystemManager>();
                    services.AddSingleton<IIngameNetworkManager, IngameNetworkManager>();
                    services.AddSingleton<INetworkManager, NetworkManager>();
                    services.AddSingleton<IVmManager, VmManager>();

                    services.AddTransient<IUserManager, UserManager>();

                    services.AddTransient<IHandshakePacketHandler, HandshakePacketHandler>();
                    services.AddTransient<IFilesystemPacketHandler, FilesystemPacketHandler>();
                    services.AddTransient<INewPlayerPacketHandler, NewPlayerPacketHandler>();

                    services.AddHostedService<Service>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<ApiStartup>();
                })
                .UseSerilog();
        }
    }
}