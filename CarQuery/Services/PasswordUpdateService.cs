using Microsoft.AspNetCore.Identity;
using Polly;

namespace CarQuery.Services
{
    public class PasswordUpdateService : IPasswordUpdateService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;

        public PasswordUpdateService(UserManager<IdentityUser> userManager, ILogger<PasswordUpdateService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<(bool Succeeded, List<string> Errors)> PasswordUpdateAsync(IdentityUser user, string newPassword)
        {
            if (user != null && !string.IsNullOrEmpty(newPassword))
            {
                //verificar se a nova senha está dentro dos parâmetros do Identity
                var passwordValidation = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, user, newPassword);

                if (!passwordValidation.Succeeded)
                {
                    return (false, passwordValidation.Errors.Select(e => e.Description).ToList());
                }

                //usado para repetir operações caso elas falhem
                var retryPolicy = Policy
                    .HandleResult<bool>(result => result == false)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

                if (await _userManager.HasPasswordAsync(user))
                {
                    IdentityResult removeResult = IdentityResult.Failed();
                    bool removePasswordResult = await retryPolicy.ExecuteAsync(async () =>
                    {
                        removeResult = await _userManager.RemovePasswordAsync(user);
                        return removeResult.Succeeded;
                    });

                    if (!removePasswordResult)
                    {
                        _logger.LogError("PasswordUpdateService (PasswordUpdateServiceAsync): erro ao remover a senha do usuário {UserId}. Erro {Error}", user.Id, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                        return (false, new[] {"Internal error"}.ToList());
                    }
                }

                IdentityResult addResult = IdentityResult.Failed();

                bool addPasswordResult = await retryPolicy.ExecuteAsync(async () =>
                {
                    addResult = await _userManager.AddPasswordAsync(user, newPassword);
                    return addResult.Succeeded;
                });

                if (!addPasswordResult)
                {
                    _logger.LogError("PasswordUpdateService (PasswordUpdateServiceAsync): erro ao adicionar senha do usuário {UserId}. Erro {Error}", user.Id, string.Join(", ", addResult.Errors.Select(e => e.Description)));
                    return (false, new[] { "Internal error" }.ToList());
                }

                return (true, Enumerable.Empty<string>().ToList());
            }
            return (false, Enumerable.Empty<string>().ToList());
        }
    }
}
