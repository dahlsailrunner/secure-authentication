using System;
using System.Security.Cryptography;
using Microsoft.AspNet.Identity;

namespace Globomantics.Framework.Identity
{
    public class CustomPasswordHasher : PasswordHasher
    {
        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Contains("|"))
            {
                var parts = hashedPassword.Split('|');
                var salt = parts[0];
                var hashedPwd = parts[1];
                if (VerifyLegacyPassword(hashedPwd, salt, providedPassword))
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
                else
                {
                    return PasswordVerificationResult.Failed;
                }
            }

            return base.VerifyHashedPassword(hashedPassword, providedPassword);
        }

        private bool VerifyLegacyPassword(string hashedPwd, string salt, string providedPassword)
        {
            var hasher = new Rfc2898DeriveBytes(providedPassword, Convert.FromBase64String(salt)) { IterationCount = 100 };
            var hashed = Convert.ToBase64String(hasher.GetBytes(128));

            return hashedPwd == hashed;
        }
    }
}