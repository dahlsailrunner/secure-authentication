﻿using System;
using System.Data;
using System.Data.Entity.SqlServer.Utilities;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Globomantics.Framework.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Globomantics.Framework
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("donotreply@globomantics.com"),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(new MailAddress(message.Destination));
            using (var client = new SmtpClient("localhost", 25))
            {
                client.Send(mailMessage);
            }
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
            var db = context.Get<IDbConnection>();
            var manager = new ApplicationUserManager(new CustomUserStore(db));
            
            // Configure custom validation
            manager.UserValidator = new CustomUserValidator(manager, db);
            manager.PasswordValidator = new CustomPasswordValidator();

            manager.RegisterTwoFactorProvider("AuthenticatorKey", new CustomAuthenticatorTokenProvider());

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(2);  //5
            manager.MaxFailedAccessAttemptsBeforeLockout = 3; //5 

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

        public override async Task<IdentityResult> AccessFailedAsync(int userId)
        {
            var user = await Store.FindByIdAsync(userId);
            if (!user.LockoutEnabled)
            {
                if (Store is IUserLockoutStore<CustomUser, int> lockoutStore)
                {
                    await lockoutStore.IncrementAccessFailedCountAsync(user);
                    await lockoutStore.UpdateAsync(user);
                    return new IdentityResult();
                }
            }
            return await base.AccessFailedAsync(userId);
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
