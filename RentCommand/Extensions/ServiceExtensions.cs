using Confluent.Kafka;
using Infrastructure.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using System.Text;

namespace RentCommandApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureApi(
           this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }

        public static IServiceCollection ConfigureMongoDb(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            services.Configure<MongoDbConfig>(configuration.GetSection("MongoDbConfig"));

            return services;
        }

        public static IServiceCollection ConfigureElasticsearchStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticConfiguration:Uri"]))
                .DefaultIndex("cars-index");

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);

            return services;
        }

        public static IServiceCollection ConfigureKafkaProducer(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.Configure<ProducerConfig>(configuration.GetSection("ProducerConfig"));

            return services;
        }

        public static IServiceCollection ConfigureVersioning(
            this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            });

            return services;
        }

        public static IServiceCollection ConfigureMongoDBHealthChecker(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddHealthChecks().AddCheck("MongoDB", () =>
            {
                string sqlHealthCheckDescription = "Tests that we can connect and select from the MongoDB database.";
                var mongoClient = new MongoClient(configuration.GetSection("MongoDbConfig:ConnectionString").Value);
                var mongoDatabase = mongoClient.GetDatabase(configuration.GetSection("MongoDbConfig:Database").Value);
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                try
                {
                    mongoDatabase.RunCommand((Command<BsonDocument>)"{ping:1}");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy(sqlHealthCheckDescription);
                }
                return HealthCheckResult.Healthy(sqlHealthCheckDescription);
            });

            return services;
        }

        public static IServiceCollection ConfigureJWT(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                    };
                });

            return services;
        }
    }
}
