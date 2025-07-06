using DynamicConfiguration.Web.Configuration.Events;
using DynamicConfiguration.Web.Configuration.Interfaces;
using DynamicConfiguration.Web.Configuration.Options;
using DynamicConfiguration.Web.Configuration.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamicConfiguration.Web
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
            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMq"));
            
            services.AddSingleton<IConfigurationStore>(_ =>
                new MongoConfigurationStore(Configuration.GetConnectionString("MongoConnection"),
                    Configuration["MongoDatabase"]));

            services.AddTransient<IConfigurationEventPublisher, ConfigurationEventPublisher>();
            
            services.AddControllersWithViews();
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

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Configuration}/{action=Index}/{id?}");
            });
        }
    }
}