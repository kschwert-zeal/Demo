using Demo.Interfaces;
using Demo.Models;
using Demo.Database;

namespace Demo.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly DBContext _db; // Your database repository

        public AuthenticationService(DBContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new user account
        /// </summary>
        public async Task<(bool success, string errorMessage, Users user)> CreateAccountAsync(Users model)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    return (false, "First name is required.", null);

                if (string.IsNullOrWhiteSpace(model.LastName))
                    return (false, "Last name is required.", null);

                if (string.IsNullOrWhiteSpace(model.UserName))
                    return (false, "User name is required.", null);

                if (string.IsNullOrWhiteSpace(model.Email) || !IsValidEmail(model.Email))
                    return (false, "Valid email address is required.", null);

                // Check if email already exists
                var existingUser = _db.Users.FirstOrDefault(x => x.Email == model.Email);
                if (existingUser != null)
                    return (false, "An account with this email already exists.", null);

                // Check if UserName already exists
                var existingUserName = _db.Users.FirstOrDefault(x => x.UserName.ToLower() == model.FirstName.Trim().ToLowerInvariant());
                if (existingUserName != null)
                    return (false, "An account with this User Name already exists.", null);

                // Validate password strength
                PasswordSecurityService passwordSecurityService = new PasswordSecurityService();
                var passwordValidation = passwordSecurityService.ValidatePasswordStrength(model.PasswordHash);
                if (!passwordValidation.isValid)
                    return (false, passwordValidation.errorMessage, null);

                // Hash the password
                var (hash, salt) = passwordSecurityService.HashPassword(model.PasswordHash);

                // Create user
                var user = new Users
                {
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    UserName = model.UserName.Trim(),
                    Email = model.Email.Trim().ToLowerInvariant(),
                    PasswordHash = hash,
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                await _db.AddAsync(user);
                await _db.SaveChangesAsync();
                return (true, string.Empty, user);
            }
            catch (Exception ex)
            {
                // Log the exception
                return (false, "An error occurred while creating the account.", null);
            }
        }

        /// <summary>
        /// Authenticates a user login
        /// </summary>
        public (bool success, string errorMessage, Users user) LoginAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return (false, "Email and password are required.", null);

                // Get user from database
                var user = _db.Users.FirstOrDefault(x => x.Email == email.Trim().ToLowerInvariant());
                if (user == null)
                    return (false, "Invalid email or password.", null);

                // Verify password
                PasswordSecurityService passwordSecurityService = new PasswordSecurityService();
                bool isPasswordValid = passwordSecurityService.VerifyPassword(password, user.PasswordHash, user.Salt);
                if (!isPasswordValid)
                    return (false, "Invalid email or password.", null);

                // Update last login time
                user.LastLoginAt = DateTime.UtcNow;
                _db.Update(user);
                _db.SaveChangesAsync();
                return (true, string.Empty, user);
            }
            catch (Exception ex)
            {
                // Log the exception
                return (false, "An error occurred during login.", null);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
