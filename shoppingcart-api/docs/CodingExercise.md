**Interview Project**

**Getting Started**

- Clone the git repo from C:\projects\interviewRepo\coeus.git into a new folder in C:\Projects
- Switch your current branch to a branch named “interview”
- Create a new branch from the interview branch named "interview_[your name]"
- From a command prompt or powershell prompt, navigate inside that directory and then one level deeper into the shoppingcart-web folder.  
- Run npm install.  Allow this to continue to run in a window 
- From another powershell window, navigate into the shoppingcart-api directory
- Run the following command: .\update-database.ps1 -CreateDatabase $true -ConnectionString "Data Source=[some server TBD];User id=sa;password=[some password];Initial Catalog=ShoppingCart"

**Backend**

- Open the coeus.sln local file that is in the base directory that just cloned.
- Modify the connection string in the appsettings to “Data Source=[some server TBD];User id=sa;password=[some password];Initial Catalog=ShoppingCart”
- Run the application and ensure it launches.  Stop running the application.
- In the Acme.ShoppingCart.Domain project, add a new domain entity with table name CustomerType.
  - Ensure this inherits from AuditableEntity.
  - `  `Following the format of the other tables, use reasonable data types and have a “CustomerTypeID” identity column, a “Name” column, a “Description” column, and a boolean column called “TaxExempt”.
  - Modify the Customer table to add a new field called CustomerTypeId to utilize this new entity and make sure any needed keys are set appropriately.  Make this a nullable field.
  - Ensure appropriate areas are modified to be able to query this from the database context later.
  - Create and apply the migration for these changes by running the add-migration.ps1 script inside the shoppingcart-api with a name for your migration - .\add-migration.ps1 $((Get-Date -Format "yyyyMMddHHmm") + "_AddCustomerType")
- Uncomment the code in the shoppingcart-api/src/Acme.ShoppingCart.Facade folder for files CustomerTypeFacade.cs and ICustomerTypeFacade.cs.  Also ucomment the code in CustomerTypeMapper.cs one level deeper in the Mappers folder
- Create a new controller for CustomerType and create an associated service to return all customer types.
- Write a test for the service method.
- Run the update-database method again from above to ensure Customer and CustomerType records exist in in your database.
- Run the shoppingcart-api project and ensure it launches to a swagger page
- Use swagger to validate that you can retrieve a list of CustomerTypes

- commit your new files and changes