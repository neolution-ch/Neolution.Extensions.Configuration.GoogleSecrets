# Neolution.Extensions.Configuration.GoogleSecrets

This package is a configuration provider that injects google secrets into the `Microsoft.Extensions.Configuration`.

It is not recommended to store secrets like connection strings in the source code directly or in environment variables. The better approach is to use a secret provider like the [secret manager](https://cloud.google.com/secret-manager) for the google cloud eco system.

## Usage

Install the package using your favourite method, for example:
`dotnet add package Neolution.Extensions.Configuration.GoogleSecrets`

Add the custom configuration provider to your `IHostBuilder `:

```c#
public static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        ...
        .ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddGoogleSecrets(options =>
            {
                options.ProjectName = "your-project-id";
            });
        });
}
```

## Options

| Name              | Required           | Description                                                                                                                                                                                                                                      | Default                                         |
| ----------------- | ------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------- |
| ProjectName       | :heavy_check_mark: | Specifies which project the secrets are read from.                                                                                                                                                                                               | `null`                                          |
| Filter            | ❌                 | A server side filter forwarded to the google api according to https://cloud.google.com/secret-manager/docs/filtering                                                                                                                             | `null`                                          |
| FilterFn          | ❌                 | A filter function that will be run after the secrets are read from google (client side filtering). Supplies the `Google.Cloud.SecretManager.V1.Secret` as parameter. If returned true the secret will be processed otherwise it will be ignored. | `secret => true`                                |
| MapFn             | ❌                 | A map function that will be run after the secrets have been filtered                                                                                                                                                                             | `secret.SecretName.SecretId.Replace("__", ":")` |
| VersionDictionary | ❌                 | By default the latest version of each secret is taken. But you can specify a specific version in the form of a dictionary. Key is the secret id and value is the version to take. For example: `{"MySecretId": "2"}`                             | `null`                                          |

## Overriding default appsettings.json values

You can override existing `appsettings.json` values by making sure the `MapFn` maps it to the correct path.
For example if we have the following `appsettings.json`:

```json
{
  "Secrets": {
    "MyGoogleSecret": "santa isn't real"
  }
}
```

We can override it by making sure that the `MapFn` maps it to `Secrets:MyGoogleSecret`. With the default `MapFn` the google secret would have to be called `Secrets__MyGoogleSecret`.

## Authentication

### Inside GCP

Authorization is done automatically when run inside google cloud.

### Outside GCP

To test it outside of google cloud you need to either set an environment variable called `GOOGLE_APPLICATION_CREDENTIALS` or use the `gcloud auth application-default login` command from the google cloud sdk.

#### Environment variable

Set the environment variable `GOOGLE_APPLICATION_CREDENTIALS` to the json file with your credentials. More details can be found here: https://cloud.google.com/docs/authentication/getting-started#setting_the_environment_variable. You can also set the environment variable via code, for example:

```c#
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\temp\\your-file.json");
```

#### gcloud auth application-default login

Simply run the command `gcloud auth application-default login` (after you have run `gcloud auth login`). This will set up the application default credentials using your personal account.

## Version History

- 1.1.5
  - Back to .NET Standard 2.1
- 1.1.4
  - Added more logging
  - Skipping unretrievalable secrets
- 1.1.3
  - Updated nuget packages
- 1.1.2
  - Fix github action to extract version from `PackageVersion` instead of `Version`
- 1.1.0
  - Migrated from .NET Standard 2.1 to .NET 6
- 1.0.1-alpha
  - Initial Release
