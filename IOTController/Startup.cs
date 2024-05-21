using Application.DAOInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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

            // Register Database
            services.AddDbContext<GreenHouseContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Register DAO services
            services.AddScoped<IGreenHouseDAO, GreenHouseEfcDAO>();
            services.AddScoped<IUserDAO, UserEfcDAO>();
            services.AddScoped<GreenHouseEfcDAO>();  

            // Register Logic services
            services.AddScoped<IGreenHouseLogic, GreenHouseLogic>();
            services.AddScoped<IUserLogic, UserLogic>();

            // Register other services
            services.AddScoped<ClientHandler>();
            services.AddScoped<GreenHouseService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

}