using System.Threading.Tasks;
using Base32;
using Microsoft.AspNet.Identity;
using OtpSharp;

namespace Globomantics.Framework.Identity
{
    public class CustomAuthenticatorTokenProvider : IUserTokenProvider<CustomUser, int>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<CustomUser, int> manager, CustomUser user)
        {
            return Task.FromResult((string)null);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<CustomUser, int> manager, CustomUser user)
        {
            var decodedKey = Base32Encoder.Decode(user.AuthenticatorKey);
            var otp = new Totp(decodedKey);
            var valid = otp.VerifyTotp(token, out _, new VerificationWindow(2, 2));

            return Task.FromResult(valid);
        }

        public Task NotifyAsync(string token, UserManager<CustomUser, int> manager, CustomUser user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<CustomUser, int> manager, CustomUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}