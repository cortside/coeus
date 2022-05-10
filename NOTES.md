TODO:

* extension methods in cortside.health for services setup
* extension methods in cortisde.domainevent for services setup
* finish up work with restsharpclient
	* logging
	* configurable retry
* cortside.aspnetcore.entityframework extension method for db setup with unit of work 
* extention method for cortside.common.cryptography EncryptionService
* cortside.aspnetcore extension method for registering types from assembly with name ending in x -- see repositoryinstaller
* update program to use webapi builder
* add services registration to webapi builder
* add configure to webapi builder
* integration test base classes/helpers/utilities in new aspnetcore package
	* use of webapi builder?
	* https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0#test-with-webapplicationfactory-or-testserver
* aspnetcore extention method for use default stuff in setup, including controller registration
* authorize controller in aspnetcore --- accesscontrol?
* someway to get common settings controller that can expose subset of appsettings???
	* bowdlerized version of appsettings?
* get master merged into develop of cortside.domainevent
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
* restsharpclient "requestId" for correlating requests and responses
* spectrum authenticator 
* async everything
* sonarcloud integration for all projects
* templates project to check dotnet new and build/test results
* add async analyzers to update-nugetpackages.ps1
	* https://www.nuget.org/packages/Microsoft.VisualStudio.Threading.Analyzers/
* if anyone hates powershell like I do and wants to run the nifty create release scripts in bash:
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

https://andrewlock.net/exploring-dotnet-6-part-3-exploring-the-code-behind-webapplicationbuilder/
https://github.com/andrewlock

https://www.hanselman.com/blog/my-ultimate-powershell-prompt-with-oh-my-posh-and-the-windows-terminal
https://blog.datalust.co/using-serilog-in-net-6/
https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0
https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio#use-startup-with-the-new-minimal-hosting-model
https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new-install
https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
