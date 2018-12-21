using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Entities;
using BISTickerAPI.Helpers;
using BISTickerAPI.Model;
using BISTickerAPI.Services;
using BISTickerAPI.Services.QTrade;
using BISTickerAPI.Services.TradeSatoshi;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace BISTickerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCommonServices(services);
            services.AddDbContext<TickerDbContext>(options => options.UseMySql(Configuration.GetConnectionString("Production")));
        }

        // This is called only on dev/debug build, instead of ConfigureServices. Hooray.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureCommonServices(services);
            services.AddDbContext<TickerDbContext>(options => options.UseMySql(Configuration.GetConnectionString("Development")));
        }

        public void ConfigureCommonServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<AppSettings>(Configuration);
            services.AddScoped<IRestClient>(s => new RestClient("https://www.cryptopia.co.nz/api"));
            services.AddScoped<ICryptopiaApi, CryptopiaAPI>();
            services.AddScoped<CryptopiaTickerService>();

            services.AddScoped<QTradeApi>();
            services.AddSingleton<QTradeRestClient>();
            services.AddScoped<QTradeTickerService>();

            services.AddScoped<TradeSatoshiAPI>();
            services.AddSingleton<TradeSatoshiRestClient>();
            services.AddScoped<TradeSatoshiTickerService>();

            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<AggregatorService>();
            services.AddScoped<MemoryCachingAggregatorService>();
            services.AddScoped<AntiDbDoSAggregatorService>();
            services.AddSingleton<CacheService>();
            services.AddHangfire(configuration => {
                configuration.UseStorage(new Hangfire.MemoryStorage.MemoryStorage());
            });
            //services.AddJsonFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseHangfireServer();
            
            if (env.IsDevelopment())
            {
                app.UseHangfireDashboard();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Console.WriteLine("Migrating database");
                var dbContext = serviceScope.ServiceProvider.GetService<TickerDbContext>();
                var appSettings = serviceScope.ServiceProvider.GetService<IOptions<AppSettings>>();
                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
                SeedData.Initialize(dbContext, appSettings.Value.Coins);
                Console.WriteLine("Migrated database");
            }

            //TODO: Is this needed for reverse proxy? Don't think so O_o
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Presentation}/{action=Index}/{id?}");
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var aggregator = serviceScope.ServiceProvider.GetService<MemoryCachingAggregatorService>();
#pragma warning disable CS0618 // Type or member is obsolete
                RecurringJob.AddOrUpdate(() => aggregator.UpdateTickers(), Cron.MinuteInterval(5));
#if DEBUG
                BackgroundJob.Schedule(() => aggregator.UpdateTickers(), TimeSpan.FromSeconds(10));
#endif
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}
