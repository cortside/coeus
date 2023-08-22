**Frontend - Continuation from Backend exercise**

- In the base directory for the project, run start-all.ps1 to run all the projects.  You should be able to make the rest of your changes while the app is running and see it automatically refresh as changes are made.
- From the same repo as above, open the shoppingcart-web folder in VS Code
- Ensure you can run this application with ng serve.
- Create a CustomerTypeResponse (look at existing CustomerResponse and OrderResponse for examples) to match what we created in the backend project.
- Modify the CustomerResponse to include a CustomerTypeResponse.
- Update the files in customer-list.component.html and customer-list.component.ts file to iterate over the results being retrieved from the shopping cart api and display a list of customers on this page
- Ensure the appropriate data including the Customer Type is being retrieved from the API.
- Bonus Points – Make a new call to get the customer types from the api and display the Name values from CustomerTypes instead of the ID values when displaying the Customer list.
- Commit your changes