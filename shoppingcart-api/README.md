# Acme.ShoppingCart

This project was created from the cortside-api template by doing the following:

```text
dotnet new --install cortside.templates
dotnet new cortside-api --name Acme.ShoppingCart --company Acme --product ShoppingCart
```

The template repo can be found at https://github.com/cortside/cortside.templates.

## Features

The template is a contrived example that was created to show a number of concepts, you have find a list of the notable [features here](Features.md).

## Service Diagram

To better understand the responsibilities and relationship of this service with others, see the [service diagram](docs/ServiceDiagram.md).

## Pre-Requisites

* .NET 6.0
* Visual Studio 2022

## Visual Studio Extensions

Here are the Visual Studio Extensions we use for this project:

- EditorConfig Language Service
- Format document on Save
- SonarLint for Visual Studio
- Extension Manager
- Code Cleanup On Save
- MappingGenerator -- https://mappinggenerator.net/

# First Time To Run

Before you run this for the first time you will need to run powershell script `.\build.ps1`

This will create the `build.json` file in:
src\Acme.ShoppingCart.WebApi\build.json

## Database location

You can override the default expected location of the database using environment variables.  The easiest way to set these is to set them up in your powershell profile (`notepad $PROFILE`):

```powershell
$env:MSSQL_SERVER="kehlstein"
$env:MSSQL_USER="sa"
$env:MSSQL_PASSWORD="password1@"
```

The default will be to use Sql Express if `$env:MSSQL_SERVER` is not set.  The default to be to use logged in user with trusted connection if `$env:MSSQL_USER` is not set.  The above example shows how to use a remote host with sql authentication enabled.

## Create database locally

Run powershell script `.\update-database.ps1 -CreateDatabase`

## Update existing local database

Run powershell script `.\update-database.ps1`

## Updating the database schema through migrations

### Add a new migration

- In Acme.ShoppingCart.Data, make changes to the database models to match the desired schema.
- To generate a new migration, run: `add-migration.ps1` and supply a name for the migration when prompted
  - also runs `Generate-Sql.ps1` and `Generate-SqlTriggers.ps1`
- If needed, edit and add to the generated C# migration file.
- If needed, run the `Generate-Sql.ps1` script to generate the SQL file.
- If needed, run the `Generate-SqlTriggers.ps1` script.
- Run the `update-database.ps1` script to update your local instance.

### Edit an existing migration

The way EntityFramework migrations work, particularly how we use them, is that editing a migration **after** it has already run in a database, will accomplish nothing. If you read the generated sql files that are run by `update-database.ps1`, you will see why. The deployment process does not ever run something like `dotnet ef down --target-migration some_previous_migration`.

That means editing existing migrations can really only be done sensibly in a branch that has not yet been merged **and preferably not yet deployed to any integrated environment like dev or stage** (or there will likely be future issues to deal with there). But know that you will need to manually delete the record in your local database's `__EFMigrationsHistory` table to run the updated script against your local db, one way or another. And undo what it did, one way or another. Deleting your local db and running `update-database.ps1 -CreateDatabase` is a viable option.

In other words, **if your branch has not yet merged AND has not yet deployed to dev/test/stage/uat/etc** then:

- delete your local `ShoppingCart` database
- delete the generated sql migration file
- update the cs migration _or_ delete the existing cs migration and add a fresh one
- repeat the remaining steps for adding a migration

`*`**NOTE:** This is one way and it's the big hammer way. Combining with the details above of migrations work, we can also rollback to a previous version, and then undo the migration changes (files, entities, etc).

## Deployment of Database changes

Database changes are deployed to shared environments when Octopus runs `update-database.ps1` against the database for the given environment being deployed to.

## Running the application locally

- `run.ps1` is a convenience script, if debugging in VisualStudio is not needed
- you may need to run `update-database.ps1` to update your local database (do not run against SqlTestOnlineApp, as deployments should do that)
- you may need to run rabbitmq in docker locally, when working with domain event message publication and consumption
  - run `start-rabbitmq.ps1` - this will start a container and configure queues and subscriptions per the config in the cloned repo
    - admin UI can be accessed at http://localhost:15672/ with admin/password as credentials

## Known problems
I came across this today while updating some of the microsoft libraries and it took me way to long to figure out -- i only noticed this from command line, which was also noticed by the docker build -- i did not see this with ncrunch but with vstest it would just show the tests as not run and no error.
The solution that i came up with was to keep Microsoft.AspNetCore.Mvc.Testing at 6.0.7 and Microsoft.NET.Test.Sdk at 17.2.0.  I tried these individually and singly the did not fix the problem but both of them together did.
Pinning to old version is not the solution, but it is an immediate fix until i can see better what is going on.
```csharp
The active test run was aborted. Reason: Error converting value "P0Y0M0DT0H0M3S" to type 'System.TimeSpan'. Path 'Payload.TestRunCompleteArgs.ElapsedTimeInRunningTests', line 1, position 289.

Test Run Aborted with error System.Exception: One or more errors occurred.
 ---> System.Exception: Error converting value "P0Y0M0DT0H0M3S" to type 'System.TimeSpan'. Path 'Payload.TestRunCompleteArgs.ElapsedTimeInRunningTests', line 1, position 289.
 ---> System.Exception: Could not cast or convert from System.String to System.TimeSpan.
   at Newtonsoft.Json.Utilities.ConvertUtils.EnsureTypeAssignable(Object value, Type initialType, Type targetType)
   at Newtonsoft.Json.Utilities.ConvertUtils.ConvertOrCast(Object initialValue, CultureInfo culture, Type targetType)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.EnsureType(JsonReader reader, Object value, CultureInfo culture, JsonContract contract, Type targetType)
   --- End of inner exception stack trace ---
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.EnsureType(JsonReader reader, Object value, CultureInfo culture, JsonContract contract, Type targetType)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, Object target)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.PopulateObject(Object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, String id)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, Object target)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.PopulateObject(Object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, String id)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, Object target)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.PopulateObject(Object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, String id)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.Deserialize(JsonReader reader, Type objectType, Boolean checkAdditionalContent)
   at Newtonsoft.Json.JsonSerializer.DeserializeInternal(JsonReader reader, Type objectType)
   at Newtonsoft.Json.JsonConvert.DeserializeObject(String value, Type type, JsonSerializerSettings settings)
   at Newtonsoft.Json.JsonConvert.DeserializeObject[T](String value, JsonSerializerSettings settings)
   at Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.JsonDataSerializer.DeserializePayload[T](Message message)
   at Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.TestRequestSender.OnExecutionMessageReceived(MessageReceivedEventArgs messageReceived, IInternalTestRunEventsHandler testRunEventsHandler)
   --- End of inner exception stack trace ---.
```
