using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
    Objective: 
        Illustrate file-uploading in a ASP-NET MVC app using vanilla 
        JavaScript and IFormFile.

    Project Info:
        Project is created using dotnet-cli with cmd:
            - dotnet new mvc --name FileUploadApp

    Packages:
        - dotnet add package Microsoft.EntityFrameworkCore --version 5.0.10
        - dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 5.0.10
        - dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 5.0.10

    Using SQL Server 2019 docker image:
        1. install Docker Desktop
        2. docker run -d --name sql_server -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=SQLServer@2019!' -p 1433:1433 mcr.microsoft.com/mssql/server:2019-latest
*/

namespace FileUploadApp
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
