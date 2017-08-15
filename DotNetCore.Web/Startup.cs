﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotNetCore.Data;
using Microsoft.EntityFrameworkCore;
using DotNetCore.Core.Infrastructure;
using DotNetCore.Core.Interface;
using System;

namespace DotNetCore.Web
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
            var conn = Configuration.GetConnectionString("DotNetCoreWeb");
            services.AddDbContextPool<WebDbContext>(options => options.UseSqlServer(conn));

            var typeFinder = new AppDomainTypeFinder();
            var typesToRegister = typeFinder.FindClassesOfType(typeof(IDependency));
            foreach (var item in typesToRegister)
            {
                var instance = (IDependency)Activator.CreateInstance(item);
                instance.Register(services);
            }

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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
