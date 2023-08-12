**Interview Project**

**Getting Started**

- Clone the git repo from <https://github.com/cortside/coeus.git> into a new folder in C:\Projects
- Switch your current branch to a branch named “interview”
- From a command prompt or powershell prompt, navigate inside that directory and then one level deeper into the shoppingcart-web folder.  
- Run npm install.  Allow this to continue to run in a window 
- From another powershell window, navigate into the shoppingcart-api directory
- Run the following command: .\update-database.ps1 -CreateDatabase $true -ConnectionString "<a name="ole_link1"></a>Data Source=[some server TBD];User id=sa;password=[some password];Initial Catalog=ShoppingCart"

**Backend**

- Open the coeus.sln local file that is in the base directory that just cloned.
- Modify the connection string in the appsettings to “Data Source=[some server TBD];User id=sa;password=[some password];Initial Catalog=ShoppingCart”
- Run the application and ensure the database is created.
- In the Acme.ShoppingCart.Domain project, add a new domain entity with table name CustomerType.
  - Ensure this inherits from AuditableEntity.
  - `  `Following the format of the other tables, use reasonable data types and have a “CustomerTypeID” identity column, a “Name” column, a “Description” column, and a boolean column called “TaxExempt”.
  - Modify the Customer table to add a new field called CustomerTypeId to utilize this new entity and make sure any needed keys are set appropriately.  You can set this to a nullable field if desired.
  - Ensure appropriate areas are modified to be able to query this from the database context later.
  - Create a migration for these changes.
  - Apply the migration.
- Create a new method in the existing customer controller and associated service to return all customer types and associated fields.
- Write a test for the service method.
- Run the update-database method again from above to ensure Customer and CustomerType records exist in in your database.

**Frontend**

- In the base directory for the cloned project, run start-all.ps1 to run all the projects.  You should be able to make the rest of your changes while the app is running and see it automatically refresh as changes are made.
- From the same repo as above, open the shoppingcart-web folder in VS Code
- Ensure you can run this application with ng serve.
- Create a CustomerTypeResponse (look at existing CustomerResponse and OrderResponse for examples) to match what we created in the backend project.
- Modify the CustomerResponse to include a CustomerTypeResponse.
- Update the files in customer-list.component.html and customer-list.component.ts file to iterate over the results being retrieved from the shopping cart api and display a list of customers on this page
- Ensure the appropriate data including the Customer Type is being retrieved from the API.
- Bonus Points – Make a new call to get the customer types from the api and display the Name values from CustomerTypes instead of the ID values when displaying the Customer list.

