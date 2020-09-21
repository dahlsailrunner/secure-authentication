# Secure Authentication
This set of projects is meant as an example of setting up 
authentication (and authorization) in ASP.NET and ASP.NET 
Core web applications.  The content here would apply to:
* RazorPages 
* MVC
* WebForms

# Getting Started
## Database
The fictional scenario is that we have an existing database and will be using it 
within the ASP.NET Identity (Core or Framework).  

If you already have an existing database instance and a way to run SQL statements from files, skip to step 3.

1. **Identify a database instance to use**
    
    To create the existing database, you can use any existing SQL Server database you 
    have handy, or set up a new instance of the database engine by installing 
    the [free Microsoft® SQL Server® 2019 Express edition](https://go.microsoft.com/fwlink/?linkid=866658).

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
## Using a `partial` class 
By using a `partial` class with the `CustomUserStore` different interfaces can 
be implemented in different files, keeping individual files smaller and focused 
on narrower areas. This approach, for example, lets us keep basic functionality 
like passwords in one file, and details of two-factor authencation in a separate 
file.

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

**.NET Framework Note:** There is a single `PasswordValidator` on the Framework 
version of the `UserManager` class. So any custom validator you create should perform
ALL of the validations you want done on passwords.