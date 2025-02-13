using Keycloak.Net;
using Keycloak.Net.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public class UsersAuthorizationService : IUsersAuthorizationService
    {
        private readonly KeycloakClient _keycloakClient;
        private readonly string _realmSchemaName;

        public UsersAuthorizationService(string restApiUrl, string username, string password, string realmSchemaName)
        {
            _realmSchemaName = realmSchemaName;
            _keycloakClient = new KeycloakClient(restApiUrl, username, password);
        }

        public async Task<Guid> CreateUser(string username, string email, string firstName, string lastName)
        {
            var result = await _keycloakClient.CreateUserAsync(_realmSchemaName, new User()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = true,
                UserName = email,
            });

            if (!result)
            {
                throw new DomainException("Failed to create user.");
            }

            var user = await GetUser(username);

            return Guid.Parse(user.Id);
        }

        public async Task<User> GetUser(string username)
        {
            var users = await _keycloakClient.GetUsersAsync(_realmSchemaName, username: username);

            var usersCount = users.Count();
            switch (usersCount)
            {
                case > 1:
                    throw new DomainException("Duplicate users found.");
                case 0:
                    throw new DomainException("No user found");
            }

            var user = users.First();

            return user;
        }

        public async Task ResetAndUpdatePassword(Guid userId, string password)
        {
            var result =
                await _keycloakClient.ResetUserPasswordAsync(_realmSchemaName, userId.ToString(), password,
                    temporary: false);

            if (!result)
            {
                throw new DomainException("Failed to update user password.");
            }
        }

        public async Task VerifyUserEmail(Guid userId)
        {
            var result = await _keycloakClient.UpdateUserAsync(_realmSchemaName, userId.ToString(), new User()
            {
                EmailVerified = true
            });

            if (!result)
            {
                throw new DomainException("Failed to verify user email.");
            }
        }
    }
}
