# Neolution.Extensions.Configuration.GoogleSecrets

This package is a configuration provider that injects google secrets into the `Microsoft.Extensions.Configuration`. It is not recommended to store secrets like connection strings in the source code directly or in environment variables. The better approach is to use a secret provider like the [secret manager](https://cloud.google.com/secret-manager) for the google cloud eco system. 



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

| Name              | Required           | Description                                                  | Default                                         |
| ----------------- | ------------------ | ------------------------------------------------------------ | ----------------------------------------------- |
| ProjectName       | :heavy_check_mark: | Specifies which project the secrets are read from.           | `null`                                          |
| Filter            | ❌                  | A server side filter forwarded to the google api according to https://cloud.google.com/secret-manager/docs/filtering | `null`                                          |
| FilterFn          | ❌                  | A filter function that will be run after the secrets are read from google (client side filtering). Supplies the `Google.Cloud.SecretManager.V1.Secret` as parameter. If returned true the secret will be processed otherwise it will be ignored. | `secret => true`                                |
| MapFn             | ❌                  | A map function that will be run after the secrets have been filtered | `secret.SecretName.SecretId.Replace("__", ":")` |
| VersionDictionary | ❌                  | By default the latest version of each secret is taken. But you can specify a specific version in the form of a dictionary. Key is the secret id and value is the version to take. For example: `{"MySecretId": "2"}` | `null`                                          |

