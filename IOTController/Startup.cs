using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
            services.AddSingleton<Server>();
            services.AddSingleton<GreenhouseManager>();
            services.AddSingleton<GreenhouseService>();

            // Register IHttpContextAccessor
            services.AddHttpContextAccessor();

            // Register HttpClient with BaseAddress
            services.AddHttpClient<Server>(client =>
            {
                client.BaseAddress = new Uri("http://dbapi:80"); // Set the base address to your API
            });
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