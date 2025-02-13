namespace Consumer.Core
{
    public class ConfigurationSections
    {
        public static string KeycloakRestApiUrl(IConfiguration configuration)
            => configuration["KeycloakIdentityServer:RestApiBaseAddress"].ToString();

        public static string KeycloakAuthority(IConfiguration configuration)
            => configuration["KeycloakIdentityServer:Authority"].ToString();

        public static string KeycloakClientId(IConfiguration configuration)
            => configuration["KeycloakIdentityServer:ClientId"].ToString();

        public static string KeycloakUsername(IConfiguration configuration)
            => configuration["KeycloakIdentityServer:UserName"].ToString();

        public static string KeycloakPassword(IConfiguration configuration)
            => configuration["KeycloakIdentityServer:Password"].ToString();

        public static string KeycloakRealmSchemaName(IConfiguration configuration)
            => configuration["KeycloakIdentityServer:RealmSchemaName"].ToString();
    }
}
