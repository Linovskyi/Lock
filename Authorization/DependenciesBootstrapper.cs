using Authorization.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Authorization
{
    public static class DependenciesBootstrapper
    {
        public static IServiceCollection AddKeycloakAuth(
            this IServiceCollection services,
            string restApiUrl, string username, string password, string realmSchemaName)
        {
            services.AddTransient<IUsersAuthorizationService>(x => new UsersAuthorizationService(
                restApiUrl, username, password, realmSchemaName));

            return services;
        }

        public static IServiceCollection EnableAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddMemoryCache();

            return services;
        }
    }
}
