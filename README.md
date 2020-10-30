# Secure Authentication
This set of projects is meant as an example of setting up 
authentication (and authorization) in ASP.NET and ASP.NET 
Core web applications.  The content here would apply to:
* RazorPages 
* MVC
* WebForms

# Getting Started
**NOTE:** This repo is the code behind my Pluralsight course [Secure User Account and Authentication Practices in ASP.NET and 
ASP.NET Core](https://app.pluralsight.com/library/courses/secure-account-authentication-practices-asp-dot-net-core).  That course evolves this code from its starting point, goes over all of the concepts / notes that you see below, and discusses security considerations for all of this - like credential stuffing, invalid redirects, password reset practices, and more. 

## Database
The fictional scenario is that we have an existing database and will be using it 
within the ASP.NET Identity (Core or Framework).  

If you already have an existing database instance and a way to run SQL statements from files, skip to step 3.

1. **Identify a database instance to use**
    
    To create the existing database, you can use any existing SQL Server database you 
    have handy, or set up a new instance of the database engine by installing 
    the [free Microsoft� SQL Server� 2019 Express edition](https://go.microsoft.com/fwlink/?linkid=866658).

2. **Identify a way to execute SQL commands** 

   SQL file(s) has been provided in the solution that can be used at various points during a walkthrough.  To apply 
   the statement in those files you need to have some kind of query interface.  Any of the options below will work:

   * [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15) (windows only)
   * [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-2017)
   * [mssql for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-mssql.mssql)
   * [sqlcmd command line interface](https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility?view=sql-server-ver15)
 
3. **Create the database and some initial data**
   
   Run the `Setup.sql` script in the database instance you have created.  This should create a new 
   database called `Globomantics` and three tables should be created and each should have a few rows of data.

   To see the data in the tables you can use the following three SQL statements:

    ```sql
    SELECT * FROM Companies
    SELECT * FROM CompanyMembers
    SELECT * FROM GlobomanticsUser
    ```

## Logging
Logging is done via [Serilog](https://github.com/serilog/serilog) to [Seq](https://datalust.co/seq).

Seq is run via its [Docker image](https://hub.docker.com/r/datalust/seq).  To run that, you 
need to have [Docker Desktop](https://www.docker.com/products/docker-desktop) installed.

Once you have Docker Desktop instaled, you can run the following commands in a terminal window to start Seq:

```
docker pull datalust/seq
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

The first command will get (pull) the Docker image for Seq onto your local machine, and 
the second runs it with all Seq services using port 5341.

If Seq is running properly you should be able to open a browser and go here;
http://localhost:5341 
You should see a Seq page up and running and now you can easily browse and explore any 
log entries that get created during development!

**NOTE:** If you want to write to a different Serilog sink, just modify the 
code in `Program.cs` in any of the projects.

## Initial Users
The database creation logic above will create the following users:
* rr@acme.com / `l00neyTunes!`
* wile@acme.com / `l00neyTunes!`
* kim@mars.com / `to1nfinity!`
* stanley@mars.com / `to1nfinity!`	

# `CustomUserStore` Notes
## Using your own table of users (existing or not)
To use your own table to store users (especially if it's an existing one), you may 
want to use a custom `IUserStore<T>` and define a class to represent your user and its 
properties - which most likely should inherit from `IdentityUser<ID>` where `ID` is the 
type for the ID of your user (default is `string` for a guid-based ID, but you could use 
`long` or `int` or something else).  The class in this project for this is `CustomUser` 
and it inherits from `IdentityUser<int>`.

You may need to "map" important properties of the `IdentityUser` class 
to your existing properties - and I've done that in this class - the `UserId` is the name of the existing database table column for the 
Id of the user, but the key column in the `IdentityUser` object is 
`Id`.  Therefore we have code like this:

```csharp
public class CustomUser : IdentityUser<int>
{
    public int UserId { get; set; }
    public string LoginName { get; set; }
    ...
    public override int Id => UserId;    
    public override string UserName => LoginName;
}
```
Then create an implementation for `IUserStore<CustomUser>` like I have 
done in the `CustomUserStore.cs` file (this is a partial class - see below).  This will have specific methods for you to create that 
will both interact with the database and get/set properties on your 
`CustomUser` object.

Then make sure to replace any of the `UseDefaultIdentity` code in `Startup` with something more like this:

```csharp
services.AddIdentityCore<CustomUser>()
    .AddSignInManager<SignInManager<CustomUser>>()
    .AddUserManager<UserManager<CustomUser>>()        
    .AddUserStore<CustomUserStore>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();
```
That wires up all of the standard identity stuff but plugs in your own custom user store.

## Using a `partial` class 
By using a `partial` class with the `CustomUserStore` different interfaces can 
be implemented in different files, keeping individual files smaller and focused 
on narrower areas. This approach, for example, lets us keep basic functionality 
like passwords in one file, and details of two-factor authencation in a separate 
file.

The schema changes that I made to the original database setup are in the numbered `Upgrade` scripts all within the `sql-scripts` directory 
and solution folder.

## Custom Password Hashing
The approach taken in these projects allows for "legacy" passwords to be validated
and then re-hashed to use the current hashing technique within ASP.NET (Core or 
Framework).  The approach is described very well by Andrew Lock in this blog post:

https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/

## Pwned Password Validation
A "pwned" (pronounced "poaned") password is one that has been compromised in some 
kind of breach and should be considered a vulnerable password.  

Both the ASP.NET Core and ASP.NET Framework projects validate new values for passwords
against the [Pwned Passwords API](https://www.troyhunt.com/ive-just-launched-pwned-passwords-version-2/).

In both cases, NuGet packages are used to facilitate the validation - which does NOT 
send actual passwords to the API but rather hashed values.

For ASP.NET Core: https://github.com/andrewlock/PwnedPasswords
For ASP.NET Framework: https://github.com/mjashton/PwnedClient

## Custom Password Validation
Just add more password validators and register them to also include those in the mix.

The `CustomPasswordValidator` is a simple example of this - and is used 
*in addition to* the NuGet-based PwnedPasswordValidator.

**.NET Framework Note:** There is a single `PasswordValidator` on the Framework 
version of the `UserManager` class. So any custom validator you create should perform
ALL of the validations you want done on passwords.

# Email Validation
At some point you will need to send out real emails to real 
email accounts using a real SMTP service, but for development 
purposes, using a fake SMTP service that "sends" emails to a simple 
email client interface where you can see and interact with them makes
testing really easy.

[smtp4dev](https://github.com/rnwood/smtp4dev) is an easy one to use 
and provides a containerized version which makes it also cross-platform.

Here are the commands to get and run it:

```
docker pull rnwood/smtp4dev
docker run --name smtp4dev -d --restart unless-stopped -e -p 3000:80 -p 2525:25 rnwood/smtp4dev
```
The comman above will let you access the SMTP server from the HOST on port 2525, and from other containers (if you are using docker compose) 
on port 25.  

And from the host (your workstation), just browse to http://localhost:3000 based on the above config and you should see an email client where you can review all emails that have been sent by this service.

Awesome stuff.  :)

# Authenticator apps (both Core and Framework projects)
Two-factor authentication is set up in both of the projects in this 
repo.

It still uses the custom database, and the schema for this new information is in the `sql-scripts/Upgrade3-TwoFactorAuth.sql` file.

**.NET Framework Note:** I didn't bother with some of the authenticator functionality (like recovery codes) in the .NET Framework project -- the AuthenticatorKey 
is just a column on the User table for this implementation.  Further, the [OtpSharp](https://www.nuget.org/packages/OtpSharp/) was used to enable the verification functionality since .NET Framework doesn't have a token provider built in.

# Customizing `UserManager`
The [base `UserManager`](https://github.com/dotnet/aspnetcore/blob/master/src/Identity/Extensions.Core/src/UserManager.cs) class has all kinds of functionality inside 
of it -- it's 2600+ lines of code as of this writing.  But you can provide some custom functionality by simply inheriting from the class
and then overriding ONLY the methods that you want/need to customize.

That's what I did for some custom logic to handle accounts that don't 
allow locking but the login page DOES allow locking.  The change is to simply continue incrementing failed login attempts and only reset them 
if a successful login occurs.

This change is shown in the `CustomUserManager` class - and don't forget that it had to replace the standard `UserManager<CustomUser>` that was registered in `Startup`.

# Request Logging with Serilog
[Request logging](https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/) (see the **"Streamlined request logging"** section) is a great feature of Serilog that includes HTTP status code and response time information for the HTTP requests that hit your website.

Including the source IP address, the user agent, and the authenticated user name if one exists in these entries is pretty simple (`Configure` method of `Startup`):

```csharp
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
    {
        diagCtx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress);
        diagCtx.Set("UserAgent", httpCtx.Request.Headers["User-Agent"]);
        if (httpCtx.User.Identity.IsAuthenticated)
        {
            diagCtx.Set("UserName", httpCtx.User.Identity?.Name);
        }
    };
});
```
# Authorization topics
## Creating a custom `IUserClaimStore`
Check out `Identity/CustomUserStore-ClaimsAndRoles.cs`  for the implementation.  What's cool about this is that I didn't create any schema behind it but am creating claims (including role claims) based on data already available.  If you are using a table for claims, that's great, but if you need to piece them together, that's fine too.

Then you can evaluate claims within your application as authorization inputs or even as inputs to database queries - as shown in the `OnGet` method of the `Members` page.

```csharp
public void OnGet()
{
    var companyId = User.Claims
        .FirstOrDefault(c => c.Type == "CompanyId")?.Value;

    var compDict = new Dictionary<int, Company>();
    var sql = @"
SELECT * 
FROM dbo.Companies c 
JOIN dbo.CompanyMembers cm 
    ON c.Id = cm.CompanyId
WHERE c.Id = @CompanyId";

    Company = _db.Query<Company, CompanyMember, Company>(sql, (c, cm) =>
    {
        if (!compDict.TryGetValue(c.Id, out var currComp))
        {
            currComp = c;
            currComp.Members = new List<CompanyMember>();
            compDict.Add(currComp.Id, currComp);
        }
        currComp.Members.Add(cm);
        return currComp;

    }, new {companyId}).FirstOrDefault();
}
```

## Role- and claims-based authorization policies
Role-based authorization is as simple as using the authorize attribute: `[Authorize(Roles = "rolename")]` or using the `User.IsInRole("rolename")` method.  

Claims-based authorization is only slightly more complex.

In `Startup->ConfigureServices`, just add claims-based policies within the `AddAuthorization` method.

```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("MfaRequired",
        p =>
        {
            p.RequireClaim("CompanyId");
            p.RequireClaim("MfaEnabled", "True");
        });
});
```

## Rights- or permissions-based authorization policy
"Rights" or "permissions" are things that can be done.  They are assigned to *roles* rather than users.  So maybe an `admin` role would be given both `ViewMembers` and `UpdateMembers` rights, but a `general` role might only be given the `ViewMembers` right.  Expand this to lots of roles and lots of rights for those roles and you get the picture.

The entire implementation for this type of authorization is in the `Authorization` folder of the ASP.NET Core project.

The following files are part of the rights-based authorization:

* `RequiredRightAttribute.cs`: defines the attribute where you can specify a named right required for the resource.
* `CustomPolicyProvider.cs`: defines a policy provider that recognizes the policy from the attribute and calls for a `RightRequirement` with the named right for authorization.
* `RightRequirement.cs`: The `IAuthorizationRequirement` that will be evaluated.  Contains the right name that is required.
* `RightRequirementHandler.cs`: Defines the logic to evaluate the requirement -- get the role from the user, and then check the list of rights given to that role.

The policy provider and the requirement handler need to be registered in `Startup`.

Then the `[RequiredRight("rightname")]` attribute can be applied wherever desired.

## MFA required or MFA challenge required policy
Two flavors here:  
* `[MfaRequired]` This attribute is the simple claims-based policy above and will ensure that a user has enrolled in two-factor authentication before being allowed to access the resource.
* `[MfaChallengeRequired]` This attribute will evaluate whether a user has passed a two-factor challenge where they needed to enter in the 6-digit code *during this login session.*  If they are enrolled but have "remembered the device" they may not have been challenged for the current login.

The first flavor is covered above.

The second flavor uses similar files as the rights-based authorization:

* `CustomPolicyProvider.cs`: Looks for the MfaChallenge policy and adds the corresponding requirement to the authZ challenge.
* `RequiresMfaChallengeAttribute.cs`: Attribute that will signify the need for an MFA challenge to access the resource.
* `MfaChallengeRequirement.cs`: Empty marker class to simply contain the requirement to be handled.
* `MfaChallengeRequirementHandler.cs`: Authorization handler that will check to see if the user has been challenged by using the `amr` claim as well as a cookie that may have been created by a custom `TwoFactorChallenge` page (below).

The handler should be registered in `Startup`.

Then to call for a challenge just use the `[RequiresMfaChallenge]` attribute.

The `Pages/TwoFactorChallenge` page contains the logic to present the challenge, and, if successful, create the cookie and redirect the user.