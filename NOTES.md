TODO:

* document RestApiClient enhancements over just RestSharp
* guard enhancements
	* https://github.com/mabroukmahdhi/Mahdhi.GuardFluently
* RestApiClient interface for configuration with openIdauthentication?
* RestApiClient from HttpClient -- for use with MockServer and TestServer
* settings controller in cortside.aspnetcore
	* have controller use SettingsModel from IoC, setup in startup
* authorization controller in cortside.aspnetcore
	* probably need way of conditionally including it, i.e. for when access control is not setup or when authorization is not setup
* use of dapper for db access with unit/integration Tests
	* https://www.learndapper.com/database-providers
	* https://www.connectionstrings.com/sqlite/
	* https://enlear.academy/how-to-write-unit-tests-with-dapper-d97f4c6f76d6
* valid mock/test jwt generation for AccessControl mocks
	* https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
* openapi injestion for mock generation in MockHttpServer
	* https://github.com/WireMock-Net/WireMock.Net/blob/master/src/WireMock.Net.OpenApiParser/WireMock.Net.OpenApiParser.csproj
* chatgpt client
	* https://www.codeproject.com/Articles/5370452/Developing-a-Client-Package-for-Integrating-OpenAI
* codeproject.ai server
	* https://www.codeproject.com/Articles/5322557/CodeProject-AI-Server-AI-the-easy-way
* dynamic filters for search
	* https://www.codeproject.com/Tips/5370451/A-Convenient-Way-of-Filtering-Objects-with-Objects
* Add concurrency example with etag/RowVersion
* for troy -- restapiclient deserialization handling with GetAsync
	* not able to know it's a deserialization error
	* class with property typed int where json has decimal, or class has non-nullable  property with null in json
	* ThrowOnAnyError = true (nothing other than serializer set in options)
* dto generator
	* https://blog.devgenius.io/net-source-generators-with-net-7-a68f29b46e74
	* https://learn.microsoft.com/en-us/dotnet/api/system.codedom.compiler.generatedcodeattribute?view=net-7.0
	* https://github.com/loresoft/EntityFrameworkCore.Generator
	* https://learn.microsoft.com/en-us/ef/core/extensions/
* commit build.config and don't update with build.ps1
	* i guess I just need to start committing that with projects, huh?
	* 11:07
	* it's expected to be overwritten by deploy -- and with the docker build stuff i have, it is in the resulting image
	* 11:07
	* that would make it easier for new people
	* 11:08
	* i just hate that it ends up being update over and over again
	* 11:08
	* i guess if build.ps1 didn't do that....it wouldn't
	* 11:08
	* hmmm
* RestApiClient does not handle well when invalid scope is configured for auth -- can't easily tell and was still calling api without a token
* Add guidelines page for expected claims
	* https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
* attribute based controller caching and locks
	* https://medium.com/@gkkomensi/learn-aop-with-c-37d9db7be6bb
	* conditional based on state?
* messages shoveled using azure portal online service bus explorer cause XmlException in DomainEventMessage.GetBody
	* https://github.com/cortside/cortside.domainevent/blob/develop/src/Cortside.DomainEvent/DomainEventMessage.cs#L33
	* https://github.com/search?q=repo%3AAzure%2Famqpnetlite%20%20getbody&type=code
	* https://github.com/Azure/amqpnetlite/blob/772d0d3b6bbc43cf64e0ccd25911317f011927d7/src/Serialization/Extensions.cs#L38
* Add message batching to DomainEvent publisher
	* https://github.com/Azure/amqpnetlite/commit/ff3ea9ea4653076c0d8e21e374f2df410952bd3d
* handling of abandoned requests from client
	* use of cancellation token?
* DbContext logging
	* https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/extensions-logging?tabs=v3
* Contract testing
	* https://github.com/pact-foundation/pact-net
	* https://medium.com/asos-techblog/pact-testing-in-net-core-6bfc5b0e9131#:~:text=For%20those%20that%20don't,when%20contracts%20change%20between%20APIs.
	* https://docs.pactflow.io/docs/examples/bi-directional/consumer/dotnet/
	* https://docs.pact.io/getting_started/comparisons
* Date only handling
	* https://stackoverflow.com/questions/21256132/deserializing-dates-with-dd-mm-yyyy-format-using-json-net
* conditionally build changed coeus directories instead of blindly building all
* better changelog generation for release prep for cortside repos
	* .\update-nugetpackages.ps1 -cortside | grep "Cortside[.]" | sort
* DB stored file/blob
	* https://github.com/cortside/c6-dcms/blob/develop/src/Domain/Entity/Blob.cs
	* https://github.com/cortside/c6-dcms/blob/develop/src/Domain/Entity/File.cs
	* https://github.com/spring2/spring2.common/tree/master/src/Spring2.Common.Storage
* new .net 8 auth
	* https://devblogs.microsoft.com/dotnet/improvements-auth-identity-aspnetcore-8/
	* https://andrewlock.net/exploring-the-dotnet-8-preview-introducing-the-identity-api-endpoints/
* RandomValues class from comms
* capture of console/log from comms-api
* EF concurrency example
* ETag example
* Add schema parameter to AddDomainEventOutbox()
	* needed by ids
* dotnet tool for sortprojectitems
	* https://github.com/KirillOsenkov/CodeCleanupTools/tree/main/SortProjectItems
	* https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create
* update identityserver4 dependencies to .net6
	* https://github.com/halloweenlv/IdentityServer4-net6
	* https://github.com/IdentityServer/IdentityServer4
	* https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation
	* https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation/compare/main...3chum:IdentityServer4.AccessTokenValidation:main
* execute sql in build
	* run tests
* add more logging to message receiver to help diagnose when queued messages are not being received and restarting service resolves issues/3700
	* https://github.com/Azure/amqpnetlite/issues/366
	* https://github.com/Azure/amqpnetlite/issues/237
	* https://github.com/Azure/amqpnetlite/issues/490  <<-- this one has good blocking information
	* https://github.com/Azure/amqpnetlite/pull/491/files <<-- updated docs
* send batch amqp messages
	* https://github.com/Azure/amqpnetlite/commit/ff3ea9ea4653076c0d8e21e374f2df410952bd3d
* dead code detector
	* https://github.com/jason-ge/DeadCodeRemover
	* https://jason-ge.medium.com/detect-and-remove-dead-code-with-roslyn-26e741b20d3c
* PAC file for browswer for local or remote docker host name resolution (local developmnent)
* middleware to log client ip
	* see ids4 GetUserIp method
	* https://stackoverflow.com/questions/49611704/application-insights-and-net-core-0-0-0-0-ip
* api template switches/flags for -- i want to receive messages, i want to publish messages, i want a db, i want example domain, auth and auth, etc
* modular templates that build on each other to create full set of possiblities
	* i.e. powershell scripts, deployment scripts
* adding update-database and/or tsqlt to build-push in deployment scripts 
* logging in entity classes
	* protected Order() {
	 // protected Order(DatabaseContext db) {
	 // logger = db.LoggerFactory?.CreateLogger<Order>();
	 // Required by EF as it doesn't know about Customer
     }
* ids mfa
	* https://github.com/damienbod/AspNetCoreHybridFlowWithApi
	* https://github.com/chsakell/aspnet-core-identity/tree/master/IdentityServer
	* https://github.com/Deblokt/IdentityServer4Demos.NETCore31/tree/master/07.%20IdentityServer4%20MFA%20%E2%80%93%20TOTP/src/IdentityServer
* logging error handling, logging of ip
	* https://mcguirev10.com/2018/02/07/serilog-dependency-injection-easy-ip-logging.html
* add link to ids readme for hashing and encoding values
	* https://dotnetfiddle.net/h3aeqd
* add example of queued work with polling for completion
	* i.e. queue some longer work, creating of some export data, that can then be checked on until it's ready
	* https://www.abhinavpandey.dev/blog/polling
	* https://farazdagi.com/posts/2014-10-16-rest-long-running-jobs/
	* https://restcookbook.com/Resources/asynchroneous-operations/
* add example of resilent handling of external async process
	* i.e. update shipping estimate on an order or creating shipment(s) from order
* updating using PATCH
	* https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-6.0
* dotnet new options for picking features in api template
* read slave aware dbcontext
* extension methods in cortside.health for services setup
* extension methods in cortisde.domainevent for services setup
* extension method to make registering client easier in restsharpclient
* cortside.aspnetcore.entityframework extension method for db setup with unit of work 
* extention method for cortside.common.cryptography EncryptionService
* cortside.aspnetcore extension method for registering types from assembly with name ending in x -- see repositoryinstaller
* add services registration to webapi builder
* add configure to webapi builder
* integration test base classes/helpers/utilities in new aspnetcore package
	* use of webapi builder?
	* https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0#test-with-webapplicationfactory-or-testserver
* aspnetcore extention method for use default stuff in setup, including controller registration
* authorize controller in aspnetcore --- accesscontrol?
* someway to get common settings controller that can expose subset of appsettings???
	* bowdlerized version of appsettings?
* domainevent retry handling without transactions
* default docker image creation in template
* healthmonitor package like sqlreport-api
* ids5
	* https://docs.duendesoftware.com/identityserver/v5/upgrades/is4_v4_to_dis_v5/
* policyserver mock docker image
* mocks docker image
* make sure that migrations work in coeus/template 
	* recreate initial migration
* review net5 to net6 change suggestions
* system test using docker images
* https://github.com/cortside/serilog.bowdlerizer/issues/4
* spectrum authenticator 
* async everything
* sonarcloud integration for all projects
* templates project to check dotnet new and build/test results
* if anyone hates powershell like I do and wants to run the nifty create release scripts in bash:
	* https://docs.microsoft.com/en-us/powershell/scripting/install/install-ubuntu?view=powershell-7.2

	```bash
	# Update the list of packages
	sudo apt-get update
	# Install pre-requisite packages.
	sudo apt-get install -y wget apt-transport-https software-properties-common
	# Download the Microsoft repository GPG keys
	wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
	# Register the Microsoft repository GPG keys
	sudo dpkg -i packages-microsoft-prod.deb
	# Update the list of products
	sudo apt-get update
	# Enable the "universe" repositories
	sudo add-apt-repository universe
	# Install PowerShell
	sudo apt-get install -y powershell
	# Start PowerShell
	pwsh
	```
* add retry to health checks
* alternate appsettings provider for consul, vault, k8s configmap, k8s secrets, etc
	* https://dev.to/engincanv/usage-of-consul-in-net-core-configuration-management-39h5
	* https://github.com/wintoncode/Winton.Extensions.Configuration.Consul
	* https://anthonychu.ca/post/aspnet-core-appsettings-secrets-kubernetes/
	* https://github.com/mrjamiebowman-blog/kubernetes-tutorial-configmap/tree/main/KubernetesTutorial
	* https://www.mrjamiebowman.com/software-development/dotnet/kubernetes-configmaps-with-net-core/
	* https://github.com/anthonychu/aspnet-core-secrets-kubernetes/

DomainEvent:
* https://github.com/Havret/dotnet-activemq-artemis-client/blob/master/src/ArtemisNetClient/Consumer.cs#L33
* https://github.com/Havret/dotnet-activemq-artemis-client/blob/master/src/ArtemisNetClient/ProducerBase.cs

Request/Response logging:
* https://github.com/dotnet/aspnetcore/issues/3700
* https://elanderson.net/2017/02/log-requests-and-responses-in-asp-net-core/
* https://github.com/exceptionnotfound/AspNetCoreRequestResponseMiddlewareDemo/blob/master/RequestResponseLoggingDemo.Web/Middleware/RequestResponseLoggingMiddleware.cs
* https://github.com/xsoheilalizadeh/raccoonLog
* https://github.com/salslab/AspNetCoreApiLoggingSample/blob/master/AspNetCoreApiLoggingSample/Middleware/ApiLoggingMiddleware.cs
* https://salslab.com/a/safely-logging-api-requests-and-responses-in-asp-net-core/
* https://blog.elmah.io/asp-net-core-request-logging-middleware/

Logging:
* https://benfoster.io/blog/serilog-best-practices/#event-type-enricher
* https://nblumhardt.com/2020/10/bootstrap-logger/
* https://esg.dev/posts/serilog-dos-and-donts/

IdentityServer:
* https://www.ashleyhollis.com/how-to-configure-azure-active-directory-with-identityserver4
	
WebApplicationFactory for integration tests:
* https://andrewlock.net/exploring-dotnet-6-part-6-supporting-integration-tests-with-webapplicationfactory-in-dotnet-6/
* https://andrewlock.net/exploring-dotnet-6-part-2-comparing-webapplicationbuilder-to-the-generic-host/	
* https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0
	
PATCH:
* https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-6.0

HttpClient:
* https://josef.codes/you-are-probably-still-using-httpclient-wrong-and-it-is-destabilizing-your-software/
* https://github.com/joseftw/JOS.HttpClient

REST file api:
* https://hub.docker.com/r/ugeek/webdav
* https://github.com/skazantsev/WebDavClient
* https://github.com/saguiitay/WebDAVClient

REST file api ideas
* https://docs.microsoft.com/en-us/rest/api/storageservices/data-lake-storage-gen2

RESTApi client:
* https://github.com/collector-bank/common-restclient/blob/master/src/Collector.Common.RestClient/RestApiClient.cs

https://andrewlock.net/exploring-dotnet-6-part-3-exploring-the-code-behind-webapplicationbuilder/
https://github.com/andrewlock

https://www.hanselman.com/blog/my-ultimate-powershell-prompt-with-oh-my-posh-and-the-windows-terminal
https://blog.datalust.co/using-serilog-in-net-6/
https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0
https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio#use-startup-with-the-new-minimal-hosting-model
https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new-install
https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/

https://github.com/stevejgordon/CorrelationId
https://github.com/collector-bank/common-correlation

https://www.singular.co.nz/2007/12/shortguid-a-shorter-and-url-friendly-guid-in-c-sharp/
https://zogface.blog/2017/08/03/using-the-tryget-pattern-in-c-to-clean-up-your-code/

https://github.com/collector-bank/backuptogit/tree/master/BackupToGit

https://github.com/collector-bank/common-swagger-extensions

TODONE:

* policyserver returns json with UpperCamelCase property names, can we have it as camelCase?
* finish up work with restsharpclient
	* logging
	* configurable retry
* update program to use webapi builder
* get master merged into develop of cortside.domainevent
* restsharpclient "requestId" for correlating requests and responses
* add async analyzers to update-nugetpackages.ps1
	* https://www.nuget.org/packages/Microsoft.VisualStudio.Threading.Analyzers/
* remove guards from cortside.domainevent
* juan's dbcontext changes
* copy config.json as config.local.json to settings/shoppingcart-web
	* update nginx startup.sh to be more like dotnet-runtime startup.sh
* validate sonar branching and prs work
	* https://www.jetbrains.com/help/teamcity/pull-requests.html#Bitbucket+Cloud+Pull+Requests
* Move MockLogger from Cortside.DomainEvent.Tests as LogEventLogger to common testutilities 
