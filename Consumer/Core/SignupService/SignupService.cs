using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Consumer.Core.SignupService
{
    public class SignupService : ISignupService
    {
        private readonly ILogger<SignupService> _logger;
        private readonly IConfiguration _configuration;
        public SignupService(ILogger<SignupService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public async Task<ServiceResponse> Login(UserLoginDto model)
        {
            var response = new ServiceResponse<string>();
            try
            {
                using (var client = new HttpClient())
                {
                    var tokenUrl = _configuration["Keycloak:TokenUrl"];
                    var grantType = _configuration["Keycloak:GrantType"];
                    var clientId = _configuration["Keycloak:ClientId"];
                    var clientParameter = _configuration["Keycloak:Client"];
                    var typeParameter = _configuration["Keycloak:Type"];
                    var login = _configuration["Keycloak:Login"];
                    var password = _configuration["Keycloak:Password"];

                    var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>(typeParameter, grantType),
                        new KeyValuePair<string, string>(login, model.Login),
                        new KeyValuePair<string, string>(password, model.Password),
                        new KeyValuePair<string, string>(clientParameter, clientId)
                    });

                    request.Content = content;
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var responseClient = await client.SendAsync(request);

                    if (responseClient.IsSuccessStatusCode)
                    {
                        var responseString = await responseClient.Content.ReadAsStringAsync();
                        var jsonDocument = JsonDocument.Parse(responseString);
                        var tokenResponse = jsonDocument.RootElement.GetProperty("access_token").GetString();
                        response.AddSuccessValue(tokenResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while executing {nameof(Login)}: {ex.Message}");
                response.AddError(ex.Message);
            }

            return response;
        }
    }
}
