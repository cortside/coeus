dotnet new --uninstall cortside.templates
dotnet new --install cortside.templates

dotnet new cortside-api --output shoppingcart-api --name Acme.ShoppingCart --force

.\merge-solutions.ps1
