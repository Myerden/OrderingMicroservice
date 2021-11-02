using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AppGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
               //.UseUrls("http://localhost:8081")
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config
                       .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                       .AddJsonFile("appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true,
                           true)
                       .AddJsonFile("ocelot.json", false, false)
                       .AddEnvironmentVariables();
               })
               .ConfigureServices(s =>
               {
                   s.AddOcelot();
                   //s.AddOcelot().AddEureka().AddCacheManager(x => x.WithDictionaryHandle());
               })
               .Configure(app =>
               {
                   app.UseRouting();

                   app.UseEndpoints(endpoints =>
                   {
                       endpoints.MapGet("/", async context =>
                       {
                           await context.Response.WriteAsync("Hello World!");
                       });
                   });

                   app.UseOcelot().Wait();
               })
               .Build();
        }
            
    }
}
