using Alachisoft.NCache.AspNetCore.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalrAPI.Hubs;
using SignalrAPI.Models;
using StackExchange.Redis;

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
            
            // Configure AppSettings 
            services.Configure<AppSettings>(Config.GetSection("AppSettings"));
            

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

            if (appSettings.IsRedisEnabled)
            {
                // Add Signal-R and Redis Backplane for Signalr Scaling
                services.AddSignalR();
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(appSettings.RedisConnectionString));
            }

            if (appSettings.IsNCacheEnabled)
            {
                // Add Signal-R Service and NCache Backplane For Signalr Scaling
                services.AddSignalR().AddNCache(ncacheOptions =>
                {
                    ncacheOptions.CacheName = appSettings.NCacheName;
                    ncacheOptions.ApplicationID = appSettings.NCacheApplicationId;
                    ncacheOptions.UserId = appSettings.NCacheUserId;
                    ncacheOptions.Password = appSettings.NCachePassword;
                });
            }
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
