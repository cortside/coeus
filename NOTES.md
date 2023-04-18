TODO:

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
