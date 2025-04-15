# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and adheres to a project-specific [Versioning](/README.md).

## [Unreleased]

### Added

- Added new overload for `AddGoogleSecrets` to allow passing `GoogleSecretsOptions`.

## [1.4.0] - 2025-03-18

### Added

- Convention to load Google Secrets project name from environment variable `GOOGLE_SECRETS_PROJECT` if not specified in the options.
- New package `Neolution.Extensions.Configuration.GoogleSecrets.AspNetCore` to reference in ASP.NET Core projects.

### Changed

- Update GitHub Actions to use the latest versions of the actions

## [1.3.1] - 2024-03-12

### Fixed

- Fixed an issue where the `GoogleSecret:SecretName:SecretVersion` syntax was not working as expected. Instead of using a starts with comparison, the code now uses a regular expression to match the secret name and version.
- Fixed an issue where the `GoogleSecret:SecretName:SecretVersion` syntax was not working as expected. The version was parsed including the `}` character. The code now removes the `}` character from the version before calling the google api.

## [1.3.0] - 2024-03-12

### Added

Added support for the `{GoogleSecret:SecretName:SecretVersion}` syntax to override the default values in the `appsettings.json` or `appsettings.{Environment}.json` files.

## [1.2.0] - 2023-06-29

### Changed

- Updated the Google.Cloud.SecretManager.V1 to the latest version to support workload identity federation

## [1.1.9] - 2023-06-29

### Added

- Changelog

[unreleased]: https://github.com/neolution-ch/Neolution.Extensions.Configuration.GoogleSecrets/compare/1.4.0...HEAD
[1.4.0]: https://github.com/neolution-ch/Neolution.Extensions.Configuration.GoogleSecrets/compare/1.3.1...1.4.0
[1.3.1]: https://github.com/neolution-ch/Neolution.Extensions.Configuration.GoogleSecrets/compare/1.3.0...1.3.1
[1.3.0]: https://github.com/neolution-ch/Neolution.Extensions.Configuration.GoogleSecrets/compare/1.2.0...1.3.0
[1.2.0]: https://github.com/neolution-ch/Neolution.Extensions.Configuration.GoogleSecrets/compare/1.1.9...1.2.0
[1.1.9]: https://github.com/neolution-ch/Neolution.Extensions.Configuration.GoogleSecrets/compare/1.1.7...1.1.9
