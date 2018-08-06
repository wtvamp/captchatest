using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotDetect.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaptchaTest
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
          services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
          services.AddMemoryCache(); // Adds a default in-memory 
                                     // implementation of 
                                     // IDistributedCache

          var corsBuilder = new CorsPolicyBuilder();
          corsBuilder.AllowAnyHeader();
          corsBuilder.AllowAnyMethod();
          corsBuilder.AllowAnyOrigin();
          corsBuilder.AllowCredentials();
          services.AddCors(options =>
          {
            options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
          });

          services.AddMvc();

          services.Configure<MvcOptions>(options =>
          {
            options.Filters.Add(new CorsAuthorizationFilterFactory("SiteCorsPolicy"));
          });

          services.AddSession(options =>
          {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
          });
    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
          app.UseCors("SiteCorsPolicy");

          if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            //configure BotDetectCaptcha
            app.UseCaptcha(Configuration);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
