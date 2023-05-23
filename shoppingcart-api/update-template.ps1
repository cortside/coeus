# make sure latest version of cortside.templates is installed
dotnet new --install cortside.templates

# update powershell scripts from root
dotnet new cortside-api-powershell --force --output ./ --name Acme.ShoppingCart --company Acme --product ShoppingCart

# update .editorconfig
dotnet new cortside-api-editorconfig --force --output ./ --name Acme.ShoppingCart --company Acme --product ShoppingCart
