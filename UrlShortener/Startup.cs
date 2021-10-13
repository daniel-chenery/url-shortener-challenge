using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UrlShortener.Business.Services;
using UrlShortener.Core.Configuration;
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

            services.Configure<DatabaseOptions>(Configuration.GetSection(DatabaseOptions.SectionName));
            services.Configure<UrlGenerationOptions>(Configuration.GetSection(UrlGenerationOptions.SectionName));
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
                app.UseExceptionHandler("/Error");
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
        }
    }
}