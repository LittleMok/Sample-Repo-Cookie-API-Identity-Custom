# Sample: Cookie based authentication with custom Identity implementation
This is an example repo for a semi-custom implementation of Identity based Authentication, supporting both Role and Permission based authorization.

Although the Permissions and Roles are a little messy and not representing of a real-life problem, it serves my purpose: To exemplify the implementation of Identity and not neccesarily to solve a _real_ problem.

It's also using serverside session, to avoid bloating the cookie size. It keeps the auth cookie size constant, while being able to store _"infinite"_ permissions and roles for the logged user.

The session storage is backed by an in-memory cache, but with few modifications it can use redis as a distributed storage.

The data storage is using PostgreSQL, but with few modifications it can run with other data providers as well.

# Solution folders
- **Controllers**
- **DataAccess**: Contains a single DBContext with an extension method to abstract the registering of the service.
- **Identity**:
	- CustomModel: Contains the custom implementation of User, Role and Permission used for both Authentication and Authorization.
	- DTO: Contains the classes used for the Auth API requests
		- LoginModel: Used for sign in
		- RegisterModel: Used for user registration. The Roles property should not exists, but being a test API it was the fastest way to add data to any user.
	- Filters: Contains the custom implementation of Permission based auth. It uses an attribute to register the roles which can access the resource. It can be used at the controller and endpoint level.
	- Managers: Contains a single file, a custom implementation of UserManager. It's not really neccesary, but I kept it because it helped me understand the workflow of the Identity Library.
	- Stores: This are the classes that retrieve and store data related to the Auth workflow with Identity.
- **Postman**: It contains a postman solution with all the requests neccesary to test the API.

# How to run
In the TestIdentity project, create a `Properties` folder, and inside a `launchSettings.json` file.

The following is an example of the expected content:
```
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:5098",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Local",
        "TestIdentity_ConnectionStrings__Default": "<Your-connection-string-here>"
      }
    }
  }
}

```