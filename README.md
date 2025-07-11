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

To override the default `appsettings.json` values you have to options which you can use together or separately:

### Use the `MapFn` to map the secret to the correct path

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

### Use the {GoogleSecret:SecretName:SecretVersion} syntax

In the `appsettings.json` you can use the `{GoogleSecret:SecretName:SecretVersion}` syntax to override the default values. For example:

```json
{
  "Secrets": {
    "MyGoogleSecret": "{GoogleSecret:MyGoogleSecret:latest}"
  }
}
```

will override the `MyGoogleSecret` with the latest version of the secret `MyGoogleSecret`.

**Note: The Version is optional and can be omitted. If omitted the latest version will be taken.**

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

## Known issues:

### Running in Alpine Linux docker container fails with "Grpc.Core.Internal.UnmanagedLibrary Attempting to load native library "/app/libgrpc_csharp_ext.x64.so"

If you run this nuget inside an alpine docker container you might get an error message similar to this:

```
System.IO.IOException: Error loading native library "/app/runtimes/linux-x64/native/libgrpc_csharp_ext.x64.so". Error loading shared library ld-linux-x86-64.so.2: No such file or directory (needed by /app/runtimes/linux-x64/native/libgrpc_csharp_ext.x64.so)
         at Grpc.Core.Internal.UnmanagedLibrary..ctor(String[] libraryPathAlternatives)
         at Grpc.Core.Internal.NativeExtension.LoadNativeMethodsUsingExplicitLoad()
         at Grpc.Core.Internal.NativeExtension.LoadNativeMethods()
         at Grpc.Core.Internal.NativeExtension..ctor()
         at Grpc.Core.Internal.NativeExtension.Get()
         at Grpc.Core.Internal.NativeMethods.Get()
         at Grpc.Core.GrpcEnvironment.GrpcNativeInit()
         at Grpc.Core.GrpcEnvironment..ctor()
         at Grpc.Core.GrpcEnvironment.AddRef()
         at Grpc.Core.Channel..ctor(String target, ChannelCredentials credentials, IEnumerable`1 options)
         at Google.Api.Gax.Grpc.GrpcCore.GrpcCoreAdapter.CreateChannelImpl(String endpoint, ChannelCredentials credentials, GrpcChannelOptions options)
         at Google.Api.Gax.Grpc.GrpcAdapter.CreateChannel(String endpoint, ChannelCredentials credentials, GrpcChannelOptions options)
         at Google.Api.Gax.Grpc.ChannelPool.GetChannel(GrpcAdapter grpcAdapter, String endpoint, GrpcChannelOptions channelOptions, ChannelCredentials credentials)
         at Google.Api.Gax.Grpc.ChannelPool.GetChannel(GrpcAdapter grpcAdapter, String endpoint, GrpcChannelOptions channelOptions)
         at Google.Api.Gax.Grpc.ClientBuilderBase`1.CreateCallInvoker()
         at Google.Cloud.SecretManager.V1.SecretManagerServiceClientBuilder.BuildImpl()
         at Neolution.Extensions.Configuration.GoogleSecrets.GoogleSecretsProvider.Load()
```

To fix it you need to add the following to your `Dockerfile` to manaully build the missing library:

```
# we need temporarly this library untill the issue is solved
# https://github.com/grpc/grpc/issues/21446#issuecomment-807990690
# can be removed once Google.Cloud.SecretManager.V1 does not relay on GRCP, there is a fix at the end as well
FROM alpine:3.13 AS grpc-build
WORKDIR /opt
RUN apk add --update alpine-sdk autoconf libtool linux-headers cmake git && \
    \
    git clone -b v1.36.4 https://github.com/grpc/grpc --depth 1 --shallow-submodules && \
    cd grpc && git submodule update --init --depth 1 && \
    \
    mkdir -p cmake/build && cd cmake/build && \
    \
    cmake -DCMAKE_BUILD_TYPE=RelWithDebInfo \
      -DgRPC_BACKWARDS_COMPATIBILITY_MODE=ON \
      -DgRPC_BUILD_TESTS=OFF \
      ../.. && \
    \
    make grpc_csharp_ext -j4 && \
    \
    mkdir -p /out && cp libgrpc_csharp_ext.* /out
```

And than at the end of your `Dockerfile` copy the outputs from the `grpc-build` stage:

```
# copy the grpc library required
COPY --from=grpc-build /out/libgrpc_csharp_ext.so /app/libgrpc_csharp_ext.x64.so
```

To speed things up you can push the `grpc-build` stage to a docker registry and pull it from there:

```
COPY --from=docker.io/your-grpc-build-image /out/libgrpc_csharp_ext.so /app/libgrpc_csharp_ext.x64.so
```

ref: https://github.com/grpc/grpc/issues/21446
