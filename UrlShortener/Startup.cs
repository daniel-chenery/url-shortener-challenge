using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Business.Services;
using UrlShortener.Core.Services;
using UrlShortener.Data.Factories;
using UrlShortener.Data.Repositories;

namespace UrlShortener
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
            services.AddControllersWithViews();

            services.AddSingleton<IUrlCharacterService, UrlCharacterService>();
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();

            services.AddScoped<IShortUrlService, ShortUrlService>();

            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ShortUrl}/{action=Create}");
            });

            // ToDo:
            // [x] Add data layer - make this a dictionary of <LongUrl, ShortUrl>
            // [x] Make sure the Url doesn't exist already
            // [x] Make the URL min 3 chars?
            // [x] Randomly generate, not sequential
            // [x] Don't generate existing URLs
            // [x] UnitTests?
            // [ ] Form with an input box (material?)
            // [ ] AJAX POST request?
        }
    }
}