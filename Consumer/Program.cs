using Authorization;
using Consumer.Controllers;
using Consumer.Core;
using Consumer.Core.SignupService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string FrontendAppCorsPolicyName = "AllowFrontendOrigin";
            const string AnyAppCorsPolicyName = "AllowAnyOrigin";

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddTransient<ISignupService, SignupService>();
            builder.Services.AddHttpClient<SignupController>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = ConfigurationSections.KeycloakAuthority(builder.Configuration);
                    options.Audience = ConfigurationSections.KeycloakClientId(builder.Configuration);
                    options.RequireHttpsMetadata = false; // TODO: Use true in production
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,

                        // Additional validation parameters as needed
                    };
                });

            builder.Services.AddKeycloakAuth(
                ConfigurationSections.KeycloakRestApiUrl(builder.Configuration),
                ConfigurationSections.KeycloakUsername(builder.Configuration),
                ConfigurationSections.KeycloakPassword(builder.Configuration),
                ConfigurationSections.KeycloakRealmSchemaName(builder.Configuration)
            );

            builder.Services.AddOpenApiDocument(document =>
            {
                document.Title = "Account API";
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseStatusCodePages();
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
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(AnyAppCorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();

            //app.UseAuthorization();


            //app.MapControllers();

            //app.Run();
        }
    }
}
