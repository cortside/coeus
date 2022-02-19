dotnet tool uninstall merge-solutions
dotnet tool install --global merge-solutions

merge-solutions /out .\coeus.sln .\shoppingcart-api\src\Acme.ShoppingCart.sln .\identityserver\src\Acme.IdentityServer.sln .\policyserver\src\PolicyServer.sln
