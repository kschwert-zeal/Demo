using Demo.Models;

namespace Demo.Interfaces
{
    public interface IAuthenticationService
    {
        Task<(bool success, string errorMessage, Users user)> CreateAccountAsync(Users model);

        public (bool success, string errorMessage, Users user) LoginAsync(string email, string password);
    }
}
