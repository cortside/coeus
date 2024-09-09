# Release 2024.09

## Changes

* Updated to net8.0
* Updated all nuget packages
* Make use of AddScopedInterfacesBySuffix and AddSingletonClassesBySuffix
	* https://github.com/cortside/cortside.common/blob/release/6.3/CHANGELOG.md
*  Use of SearchBuilder classes now located in Cortside.AspNetCore
* Introduction of TestUtilities project with builder classes for dtos, models and entities
* Make use of WebApiFixture<T> for setting up integration tests
	* Allows for simpler test setup and for simpler configuration by only overriding what is needed
* Convert to using Asp.Versioning
