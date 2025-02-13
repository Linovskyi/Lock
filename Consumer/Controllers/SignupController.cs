using Consumer.Core.SignupService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignupController : ControllerBase
    {
        private readonly ILogger<SignupController> _logger;
        private readonly ISignupService _signupService;
        public SignupController(ILogger<SignupController> logger, ISignupService signupService)
        {
            _logger = logger;
            _signupService = signupService;
        }

        [HttpPost("get-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            var result = await _signupService.Login(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Errors);
        }
    }
}
