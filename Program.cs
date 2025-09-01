using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using sarw_rp.Models;
using System.Reflection;

namespace sarw_rp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            //var connectionString = Environment.GetEnvironmentVariable("sarwdb");
            //builder.Services.AddDbContext<SarwrpdbContext>(options =>options.UseSqlServer(connectionString));
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
                                                                      // Other options like ValueLengthLimit, MultipartHeadersLengthLimit can also be set here
            });
            builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });        // Adds Swagger generator

            var app = builder.Build();

            // Enable middleware for Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(); // Optional: customize with SwaggerUI options
            }

            // Configure the HTTP request pipeline.
            app.UseCors(configurePolicy: policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();

            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
