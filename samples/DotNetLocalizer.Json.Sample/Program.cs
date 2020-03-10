using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetLocalizer.Json.Sample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = Program.BuildWebHost(args);

            host.Run();
        }

        private static IHost BuildWebHost(string[] args)
        {
            return CreateHostBuilder(args).Build();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>()).ConfigureLogging((context, logging) =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
        }
    }
}