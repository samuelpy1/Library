using System.Reflection;
using library_system.Application.Services;
using library_system.Domain.Interfaces;
using library_system.Infrastructure;
using library_system.Infrastructure.Persistence.Context;
using library_system.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace library_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = builder.Configuration["Swagger:Title"],
                        Description = builder.Configuration["Swagger:Description"],
                        Contact = new OpenApiContact()
                        {
                            Name = "Library System",
                            Email = "library@fiap.com.br",
                        },
                    }
                );

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    x.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Enable Swagger UI in all environments (useful for App Service)
            app.UseSwagger();
            app.UseSwaggerUI();

            // Only redirect to HTTPS during Development to avoid issues on Linux App Service
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet(
                "/",
                context =>
                {
                    context.Response.Redirect("/swagger", permanent: false);
                    return Task.CompletedTask;
                }
            );

            app.Run();
        }
    }
}