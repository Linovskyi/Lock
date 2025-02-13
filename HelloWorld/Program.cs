using HelloWorld.Core;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using RedLockNet;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Authorization;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string FrontendAppCorsPolicyName = "AllowFrontendOrigin";
            const string AnyAppCorsPolicyName = "AllowAnyOrigin";

            var builder = WebApplication.CreateBuilder(args);

            var multiplexer = new List<RedLockMultiplexer> { ConnectionMultiplexer.Connect("localhost") };
            var redlockFactory = RedLockFactory.Create(multiplexer);
            builder.Services.AddSingleton<IDistributedLockFactory>(redlockFactory);

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = ConfigurationSections.KeycloakAuthority(builder.Configuration);
                    options.Audience = ConfigurationSections.KeycloakClientId(builder.Configuration);
                    options.RequireHttpsMetadata = false; // Use true in production
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,

                        // Additional validation parameters as needed
                    };
                });

            builder.Services.EnableAuthorization();
            builder.Services.AddControllers();

            builder.Services.AddKeycloakAuth(
                ConfigurationSections.KeycloakRestApiUrl(builder.Configuration),
                ConfigurationSections.KeycloakUsername(builder.Configuration),
                ConfigurationSections.KeycloakPassword(builder.Configuration),
                ConfigurationSections.KeycloakRealmSchemaName(builder.Configuration)
            );

            builder.Services.AddOpenApiDocument(document =>
            {
                document.Title = "Documentation";
                document.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description = "Paste your JWT token into the input field.",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(FrontendAppCorsPolicyName,
                    builder => builder.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AnyAppCorsPolicyName,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.Lifetime.ApplicationStopping.Register(() => { redlockFactory.Dispose(); });

            app.UseOpenApi();
            app.UseSwaggerUi(config => config.TransformToExternalPath = (internalUiRoute, request) =>
            {
                if (internalUiRoute.StartsWith("/") == true &&
                    internalUiRoute.StartsWith(request.PathBase) == false)
                {
                    return request.PathBase + internalUiRoute;
                }
                else
                {
                    return internalUiRoute;
                }
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
