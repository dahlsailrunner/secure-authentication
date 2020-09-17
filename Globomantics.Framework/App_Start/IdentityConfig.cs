using System;
using System.Data;
using System.Data.Entity.SqlServer.Utilities;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Globomantics.Framework.Models;

namespace Globomantics.Framework
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<CustomUser, int>
    {
        public ApplicationUserManager(IUserStore<CustomUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new CustomUserStore(context.Get<IDbConnection>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<CustomUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<CustomUser, int>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<CustomUser, int>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            manager.PasswordHasher = new CustomPasswordHasher();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<CustomUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        protected override async Task<bool> VerifyPasswordAsync(IUserPasswordStore<CustomUser, int> store, CustomUser user,
            string password)
        {
            var hash = await store.GetPasswordHashAsync(user).WithCurrentCulture();
            var pwdResult = PasswordHasher.VerifyHashedPassword(hash, password);

            if (pwdResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var newHashedPwd = PasswordHasher.HashPassword(password);
                await store.SetPasswordHashAsync(user, newHashedPwd);
                await store.UpdateAsync(user);
            }

            return pwdResult != PasswordVerificationResult.Failed;
        }
    }

    public class ApplicationSignInManager : SignInManager<CustomUser, int>
    {
        private readonly ApplicationUserManager _userManager;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        {
            _userManager = userManager;
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(CustomUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
