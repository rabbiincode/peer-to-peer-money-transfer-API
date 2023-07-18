using Microsoft.OpenApi.Models;
using System.Reflection;
using peer_to_peer_money_transfer.BLL.Extensions;

namespace peer_to_peer_money_transfer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.             
            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization();
           
            builder.Services.AddAutoMapper(Assembly.Load("peer_to_peer_money_transfer.Shared"));
            builder.Services.AddHttpContextAccessor();

            builder.Services.RegisterServices();
            builder.Services.AddDatabaseConnection();
            builder.Services.AddJwtAuthentication();
            builder.Services.AddPolicyAuthorization();

            builder.Services.ConfigureEmailServices();
            builder.Services.AddHttpClient("SmsClient", client =>
            {
                client.BaseAddress = new Uri("https://www.bulksmsnigeria.com/api/v1/sms/create");
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:4200", "https://localhost:4200")
                 );
            });

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the bearer scheme.
                                    Please Enter 'Bearer' [space] and then your token
                                    Example: Bearer 123456",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new string[]{}
                    }
                });

                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "CashMingle", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            // custom exception handler
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}