# Secure Authentication
This set of projects is meant as an example of setting up 
authentication (and authorization) in ASP.NET and ASP.NET 
Core web applications.  The content here would apply to:
* RazorPages 
* MVC
* WebForms

# Getting Started
## Database

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