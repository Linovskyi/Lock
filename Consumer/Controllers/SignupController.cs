using Consumer.Controllers.HelloWorld;
using Consumer.Core.SignupService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Consumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignupController : ControllerBase
    {
        private readonly ILogger<SignupController> _logger;
        private readonly ISignupService _signupService;
        private readonly HttpClient _httpClient;

        public SignupController(ILogger<SignupController> logger, ISignupService signupService, HttpClient httpClient)
        {
            _logger = logger;
            _signupService = signupService;
            _httpClient = httpClient;
        }

        [HttpPost("get-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            var result = await _signupService.Login(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Errors);
        }

        [Authorize]
        [HttpPost("get-auth-hello-world")]
        public async Task<IActionResult> GetHelloWorld([FromBody] TokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    _logger.LogWarning("Token is missing in request body.");
                    return BadRequest("Token is required.");
                }

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7194/HelloWorld");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch HelloWorld. Status: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Failed to fetch HelloWorld.");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var helloWorldResponse = JsonSerializer.Deserialize<HelloWorldResponseDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Ok(helloWorldResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calling HelloWorld API.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
