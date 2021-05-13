using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Angular.SSR.Example
{
    public class Startup
    {
        internal class FS
        {
            public static string ClientApp = "ClientApp";
            public static string ngDist = "dist";
            public static string browser = "browser";
            public static string server = "server";

        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = FS.ClientApp+'/'+FS.ngDist;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            string dirClientApp = Path.Combine(Directory.GetCurrentDirectory(), FS.ClientApp);
            string ngdistPath = Path.Combine(dirClientApp, FS.ngDist, FS.browser);
#if false
            app.UseSpaStaticFiles();
            
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(ngdistPath),
               // RequestPath = new Microsoft.AspNetCore.Http.PathString("")
            });
#endif
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(dirClientApp, FS.ngDist, FS.browser))
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.StartupTimeout = new System.TimeSpan(0, 1, 59);
                spa.Options.SourcePath = FS.ClientApp;
                spa.Options.DefaultPage = '/'+FS.browser+"/index.html";

                //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");

#pragma warning disable CS0618 // Type or member is obsolete
                spa.UseSpaPrerendering(options =>
                {
                    //options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.js";
                    options.BootModulePath = Path.Combine(spa.Options.SourcePath, FS.ngDist, FS.server, "main.js"); ;
                    AngularCliBuilder angularBuilder = null;
                    options.BootModuleBuilder = null;

                    if(env.IsDevelopment())
                    {
                        angularBuilder = new AngularCliBuilder(npmScript: "build:ssr");
                    }
                    options.BootModuleBuilder = angularBuilder;

                    options.ExcludeUrls = new[] { "/sockjs-node" };
                });
#pragma warning restore CS0618 // Type or member is obsolete

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
