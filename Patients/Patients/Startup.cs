using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace Patients
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Este método se utiliza para agregar servicios al contenedor de inyección de dependencias.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Configuración de Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Patient API",
                    Description = "API para gestionar pacientes de una clínica",
                    Contact = new OpenApiContact
                    {
                        Name = "Clinic API",
                        Email = "clinic@example.com",
                        Url = new Uri("https://example.com"),
                    }
                });
            });
        }

        // Este método se utiliza para configurar el pipeline de solicitudes HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Registrar en la consola y en un archivo *.log en entorno de desarrollo
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs\\devlog-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            else
            {
                // Registrar solo en un archivo *.log en entorno de calidad
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("logs\\qalog-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }

            app.UseSerilogRequestLogging(); // Agregar middleware para registrar solicitudes HTTP

            app.UseRouting();
            app.UseAuthorization();

            // Configuración de Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
