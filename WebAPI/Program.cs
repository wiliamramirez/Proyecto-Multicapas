using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostserver = CreateHostBuilder(args).Build();

            using (var ambiente = hostserver.Services.CreateScope())
            {
                var services = ambiente.ServiceProvider;

                try
                {
                    // Agregar data de prueba
                    var userManager = services.GetRequiredService<UserManager<Usuario>>();

                    var context = services.GetRequiredService<SistemaDbContext>();
                    context.Database.Migrate();


                    DataAdmin.InsertData(context, userManager).Wait(); // Wait() -> para hacerlo asincrono
                }
                catch (Exception e)
                {

                    var logging = services.GetRequiredService<ILogger<Program>>();
                    logging.LogError(e, "Ocurrio un error en la migracion");
                }

                hostserver.Run();

            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
