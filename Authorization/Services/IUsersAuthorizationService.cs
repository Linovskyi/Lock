using Keycloak.Net.Models.Users;

namespace Authorization.Services
{
    public interface IUsersAuthorizationService
    {
        Task<Guid> CreateUser(string username, string email, string firstName, string lastName);

        Task ResetAndUpdatePassword(Guid userId, string password);

        Task VerifyUserEmail(Guid userId);

        Task<User> GetUser(string username);


    }
}
