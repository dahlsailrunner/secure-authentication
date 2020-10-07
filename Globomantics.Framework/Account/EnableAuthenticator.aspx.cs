using System;
using System.Web;
using Base32;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OtpSharp;

namespace Globomantics.Framework.Account
{
    public partial class EnableAuthenticator : System.Web.UI.Page
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var secretKeyBytes = KeyGeneration.GenerateRandomKey(20);
                SharedKey = Base32Encoder.Encode(secretKeyBytes);

                var encodedIssuer = HttpUtility.UrlEncode("GlobomanticsFramework");
                SharedKeyField.Value = SharedKey;
                AuthenticatorUri = KeyUrl.GetTotpUrl(secretKeyBytes, User.Identity.Name) + "&issuer=" + encodedIssuer;
            }
        }

        protected void Verify(object sender, EventArgs e)
        {
            var secretBytes = Base32Encoder.Decode(SharedKeyField.Value);
            var otp = new Totp(secretBytes);
            var providedCode = CodeVerification.Text;
            var correctCode = otp.VerifyTotp(providedCode.Trim(), out _, new VerificationWindow(2, 2));

            if (correctCode)
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = manager.FindById(Convert.ToInt32(User.Identity.GetUserId()));
                user.TwoFactorEnabled = true;
                user.AuthenticatorKey = SharedKeyField.Value;
                manager.Update(user);

                Response.Redirect("/Account/TwoFactorAuthentication", true);
            }
            else
            {
                FailureText.Text = "Invalid verification code.";
                ErrorMessage.Visible = true;
            }
        }
    }
}