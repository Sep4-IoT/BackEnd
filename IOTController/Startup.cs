using System.Net.Sockets;
using Application.DAOInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IOTController
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddSingleton<TcpClient>();
            services.AddSingleton<ClientHandler>();
            services.AddSingleton<GreenHouseManager>(); 
            services.AddSingleton<GreenhouseService>();
            services.AddSingleton<IGreenHouseLogic, GreenHouseLogic>();
            services.AddSingleton<IGreenHouseDAO, GreenHouseEfcDAO>();
            services.AddDbContext<GreenHouseContext>();
            services.AddSingleton<IUserDAO, UserEfcDAO>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}