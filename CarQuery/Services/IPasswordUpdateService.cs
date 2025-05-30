using Microsoft.AspNetCore.Identity;

namespace CarQuery.Services
{
    public interface IPasswordUpdateService
    {

        public Task<(bool Succeeded, List<string> Errors)> PasswordUpdateAsync(IdentityUser user, string newPassword);
    }
}
