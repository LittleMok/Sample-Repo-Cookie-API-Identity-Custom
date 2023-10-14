# Sample Project: Cookie based authentication with custom Identity implementation
This is an example repo for a semi-custom implementation of Identity based Authentication, supporting both Role and Permission based authorization.

Although the Permissions and Roles are a little messy and not representing of a real-life problem, it serves my purpose: To exemplify the implementation of Identity and not neccesarily to solve a _real_ problem.

It's also using serverside session, to avoid bloating the cookie size. It keeps the auth cookie size relatively small, while being able to store _"infinite"_ permissions and roles for the logged user.

The session storage is backed by redis as a distributed storage.

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

# How to run (as project, from Visual Studio)
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

# How to run (As docker-compose project, replicated)
Located in the project root, build the API image with `docker-compose -f ./docker-compose-build.yaml build`

Create a `.env` file, following the `.env.example` in the project root.

You should be able to run the project with `docker-compose -f ./docker-compose.yaml up`. It contains a `redis` service, two containers for the API and a `nginx` container to serve as a reverse proxy.

To call the API, you need to modify your `hosts` file, adding the `api.identity.local` alias to the `127.0.0.1` address

Example, from my own hosts file
```
127.0.0.1 localhost local cache api.identity.local
```

Having done that, you should be able to call the API. Check the Postman collection first, and ensure you're using the proper URL. It has two configured as variables.

One is for the API running as a dotnet standalone project (http://localhost:5098) and the other is for when you're running behind nginx (http://api.identity.local)
