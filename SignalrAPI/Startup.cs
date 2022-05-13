using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalrAPI.Hubs;
using SignalrAPI.Models;

namespace SignalrAPI
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup(IConfiguration _Config, IWebHostEnvironment env)
        {
            Config = _Config;
            var builder = new ConfigurationBuilder()
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Config = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Config.GetSection("AppSettings").Get<AppSettings>();
            services.AddControllers();

            // Add Cors Policy
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                //.SetIsOriginAllowed(_ => true);
                .WithOrigins(appSettings.OmnizantDevWebHost, appSettings.OmnizantPMAWebHost);
            }));

            // Add Signal-R Service and Radis For Load Balancer
            services.AddSignalR().AddStackExchangeRedis("127.0.0.1");

            // Mapping AppSettings Configuration
            services.Configure<AppSettings>(Config.GetSection("AppSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<EmailHub>("/emailhub");
            });
        }
    }
}
