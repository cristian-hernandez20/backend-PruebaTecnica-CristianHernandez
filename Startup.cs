global using System.ComponentModel.DataAnnotations.Schema;
global using System.ComponentModel.DataAnnotations;
global using System.Text.Json.Serialization;
global using System.Globalization;


global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;

global using AutoMapper;
global using Interfaces;
global using Services;
global using Mappers;
global using Models;

using Middlewares;
using System.Net;

namespace ruleta {
    public class Startup(IConfiguration configuration) {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigurationServices(IServiceCollection services) {
            Console.WriteLine($"Start services, CPUs {Environment.ProcessorCount}");

            ConfigureCors(services);
            ConfigureDatabase(services);
            ConfigureScopedServices(services);

            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Startup));

            /*  50 MB in bytes */
            services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; });
        }
        private void ConfigureCors(IServiceCollection services) {

            string server_ip = GetLocalIPAddress();
            services.AddCors(options => {
                options.AddPolicy("AllowAnyOrigin", builder => {
                    var allowed_origins = Configuration.GetSection("IpAccess").Get<string[]>();
                    var origins = new List<string>(allowed_origins)
                    {
                        $"http://{server_ip}:8080", $"http://{server_ip}",
                        $"https://{server_ip}:8080", $"https://{server_ip}"
                    };
                    builder.WithOrigins([.. origins]).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
        }
        private void ConfigureDatabase(IServiceCollection services) {
            services.AddDbContext<DataContext>(options => {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
        }
        private static void ConfigureScopedServices(IServiceCollection services) {
            ConfigureGeneralesServices(services);
            ConfigureGeneralesMappers(services);
        }
        private static void ConfigureGeneralesServices(IServiceCollection services) {
            services.AddScoped<IResultServices, ResultServices>();
            services.AddScoped<IUserServices, UserServices>();
        }
        private static void ConfigureGeneralesMappers(IServiceCollection services) {
            services.AddAutoMapper(typeof(UserMap));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.Use(async (context, next) => await next());

            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAnyOrigin");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.Run(async (context) => {
                if (context.Request.Method == "GET" && context.Request.Path == "/") {
                    await context.Response.WriteAsync("Api on");
                }
                else {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("No found route");
                }
            });
        }
        private static string GetLocalIPAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("No se pudo encontrar la dirección IP local.");
        }
    }
}
