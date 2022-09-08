# Neolution GoogleSecrets for ASP.NET

Inside `Program.cs` add `ConfigureGoogleSecrets()` to the WebHost builder.

    private static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .ConfigureGoogleSecrets() // <--
            .UseStartup<Startup>();
    }

When you want to load the secrets on application start, add the ProjectID assigned to your Google Secrets Manager with the following environment variable to the system:

    GOOGLE_SECRETS_PROJECT_ID

## On development machines

Add the environment variable to the launchSettings.json file. E.g.

    {
      "profiles": {
        "IIS Express": {
          "commandName": "IISExpress",
          "launchBrowser": true,
          "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development",
            "GOOGLE_SECRETS_PROJECT_ID": "your-gcloud-project-id" // <--
          }
        }
      }
    }

## On Google Cloud Run

Create a new revision of your web application and add a environment variable named `GOOGLE_SECRETS_PROJECT_ID` with your ProjectID as the value.