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

namespace ruleta {
    public class Startup(IConfiguration configuration) {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigurationServices(IServiceCollection services) {
            Console.WriteLine($"Start services, CPUs {Environment.ProcessorCount}");

            ConfigureCors(services);
            ConfigureDatabase(services);
            ConfigureAuthentication(services);
            ConfigureScopedServices(services);

            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Startup));

            services.AddSignalR();

            /*  50 MB in bytes */
            services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; });
        }
        private void ConfigureCors(IServiceCollection services) {
            services.AddCors(options => {
                options.AddPolicy("AllowAnyOrigin", builder => {
                    builder.WithOrigins(Configuration.GetSection("IpAccess").Get<string[]>())
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
        }
        private void ConfigureDatabase(IServiceCollection services) {
            services.AddDbContext<DataContext>(options => {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
        }
        private void ConfigureAuthentication(IServiceCollection services) {
            var token_value = Configuration.GetSection("AppSettings:Token").Value;
        }
        private static void ConfigureScopedServices(IServiceCollection services) {
            ConfigureGeneralesServices(services);
            ConfigureGeneralesMapping(services);
        }
        private static void ConfigureGeneralesServices(IServiceCollection services) {
            /* Services */
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IResultServices, ResultServices>();
        }
        private static void ConfigureGeneralesMapping(IServiceCollection services) {
            /* Mappers */
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
                    await context.Response.WriteAsync("Ruta no encontrada");
                }
            }
            );
        }
    }
}
