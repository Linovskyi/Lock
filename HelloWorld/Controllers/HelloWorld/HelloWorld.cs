using HelloWorld.Controllers.HelloWorld.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedLockNet;

namespace Hello_World.Controllers.HelloWorld
{
   
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
        private readonly ILogger<HelloWorldController> _logger;
        private readonly IDistributedLockFactory _LockFactory;

        public HelloWorldController(ILogger<HelloWorldController> logger, IDistributedLockFactory lockFactory)
        {
            _logger = logger;
            _LockFactory = lockFactory;
        }
        [Authorize]
        [HttpGet(Name = "GetHelloWorld")]
        public async Task<HelloWorldResponse> GetHelloWorld()
        {
            await using var _lock = await _LockFactory.CreateLockAsync(typeof(HelloWorldResponse).Name, TimeSpan.FromSeconds(10));


            if (_lock.IsAcquired)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                var responseObject = new HelloWorldResponse
                {
                    Message = "Hello, World!",
                    Timestamp = DateTime.UtcNow
                };

                return responseObject;
            }

            throw new InvalidOperationException("Resource is locked right now. Try again later!");
        }
    }

}