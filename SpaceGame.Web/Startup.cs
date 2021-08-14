using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SpaceGame.Business.Hubs;
using SpaceGame.Web.Extensions;

namespace SpaceGame.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("AllowAll", p =>
                p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().SetPreflightMaxAge(TimeSpan.FromSeconds(86400))));

            services.AddMySingleton();

            services.AddMyScoped();

            services.AddMyTransient();

            services.AddHttpContextAccessor();

            services.AddControllers();
            
            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseDefaultFiles();
            
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/dashboard", async context =>
                {
                    await context.Response.SendFileAsync("wwwroot/dashboard.html");
                });
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gameHub");
                endpoints.MapHub<DashboardHub>("/dashboardHub");
            });
        }
    }
}