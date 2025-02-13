namespace Consumer.Core.SignupService
{
    public interface ISignupService
    {
        Task<ServiceResponse> Login(UserLoginDto model);

    }
}
