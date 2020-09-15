using System;
using System.Security.Cryptography;
using Globomantics.Core.IndAcc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Globomantics.Core.IndAcc.Areas.Identity
{
    public class CustomPasswordHasher : PasswordHasher<CustomUser>
    {
        public override PasswordVerificationResult VerifyHashedPassword(CustomUser user, string hashedPassword, string providedPassword)
        {
            if (!string.IsNullOrEmpty(user.PasswordSalt))
            {
                if (VerifyLegacyPassword(Convert.FromBase64String(user.PasswordSalt), 
                    hashedPassword, providedPassword))
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }

                return PasswordVerificationResult.Failed;
            }

            return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

        private bool VerifyLegacyPassword(byte[] salt, string hashedPassword, string providedPassword)
        {
            var hasher = new Rfc2898DeriveBytes(providedPassword, salt) { IterationCount = 1000 };
            var hashedVersionOfProvidedPassword = Convert.ToBase64String(hasher.GetBytes(128));
            return hashedPassword == hashedVersionOfProvidedPassword;
        }
    }
}
