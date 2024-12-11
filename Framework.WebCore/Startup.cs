using Framework.Core.Abstractions;
using Framework.Core.Extensions;
using Framework.Core.UOW;
using Framework.WebCore.Extensions;
using Framework.WebCore.Filters;
using Framework.WebCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Framework.WebCore
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
            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.UseSqlServerDataAccessor(); 
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();  
        }
    }
}
