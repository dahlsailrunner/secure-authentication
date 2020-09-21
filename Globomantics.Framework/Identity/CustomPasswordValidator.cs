using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using PwnedClient;

namespace Globomantics.Framework.Identity
{
    public class CustomPasswordValidator : PasswordValidator
    {
        public CustomPasswordValidator()
        {
            RequiredLength = 6;
            RequireNonLetterOrDigit = true;
            RequireDigit = true;
            RequireLowercase = true;
            RequireUppercase = true;
        }

        public override async Task<IdentityResult> ValidateAsync(string password)
        {
            var baseResult = await base.ValidateAsync(password);
            if (baseResult != IdentityResult.Success)
            {
                return baseResult;
            }

            var globResult = ValidatePasswordDoesntUseGlobomantics(password);
            if (globResult != IdentityResult.Success)
            {
                return globResult;
            }

            var pwnedResult = await ValidatePasswordHasNotBeenPwned(password);
            if (pwnedResult != IdentityResult.Success)
            {
                return pwnedResult;
            }

            return IdentityResult.Success;
        }

        private IdentityResult ValidatePasswordDoesntUseGlobomantics(string password)
        {
            if (password.ToLowerInvariant().Contains("glob"))
            {
                return IdentityResult.Failed(
                    "Variants of Globomantics cannot be used in a password.");
            }

            return IdentityResult.Success;
        }

        private async Task<IdentityResult> ValidatePasswordHasNotBeenPwned(string password)
        {
            const int considerPwned = 1000;
            var pwnChecker = new PasswordChecker();

            var pwnedCount = await pwnChecker.GetBreachCountAsync(password);

            return (pwnedCount >= considerPwned)
                ? new IdentityResult($"Cannot use password that have been pwned more than {considerPwned} times.")
                : IdentityResult.Success;
        }
    }
}
