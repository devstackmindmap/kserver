using System;
using System.Threading;
using AkaThreading;
using AkaUtility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            AntiDuplicator.AppRunning();
            CreateWebHost(args).Run();
        }

        public static IWebHost CreateWebHost(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            if (args.Length < 1 || (args[0].In("Review", "Live") && args.Length < 2))
            {
                throw new ArgumentException("Argument length > 1 or  review, live length > 2 ");
            }

            var runmode = args[0];

            var version = args.Length > 1 ? args[1] : null;

            Console.WriteLine(runmode);
            Console.WriteLine(version);


            var host=  WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseEnvironment(runmode)
                .UseSetting("DataVersion", version)
                .UseStartup<StartUp>()
                .Build();

            return host;

        }
    }
}
