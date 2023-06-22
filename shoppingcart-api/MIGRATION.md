# Migration notes and instructions

## 2023.01 - API

### New
* Addition of .gitattributes with more explicit handling set

### Breaking Changes

* Changes to ApplicationInsights registration
	* Serilog:WriteTo -- add connectionString property as emtpy string to ApplicationInsights element
	* ApplicationInsights section should replace InstrumentationKey with ConnectionString
* Use of Cortside.Health ServiceCollections extentions for health registration
	* https://github.com/cortside/coeus/tree/develop/shoppingcart-api#database-location

### Changes

* Updates to database related scripts for better handling different database editions and location.  See notes here:
	* https://github.com/cortside/coeus/tree/develop/shoppingcart-api#database-location
	* 
* Updates to how Generate-SqlTriggers.ps1 works to better support other sql editions as well as remote server.  Replaces use of stored procedures that wrote out generated files by doing text generation in powershell.
* More implementation of business logic examples in domain entities, domain services and facades
* Integration tests resolve Json serializer settings from services for use in serializing as opposed to relying on global serializer settings.  This is to resolve conflicts in latest test libraries from microsoft that don't handle custom serializer classes from newtonsoft.

## 2023.01 - Web

* continued work in progress for initial full examples
