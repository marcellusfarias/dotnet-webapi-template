# Dotnet WebAPI Template

This project aims to be a template for new WebAPI projects. It already sets up:

Tech stack:
* .Net 7 (migrating to .Net 8 soon)
* Swagger
* Postgres with Entity Framework Core
* Docker and Docker Swarm
* xUnit, AutoFixture, NSubstitute and TestContainers.

## Features implemented

### Architecture and DDD

This project adopts what many .Net influencers commonly refer to as "Clean Architecture" with Domain-Driven Design (DDD). The organizational structure you'll encounter draws inspiration from the accumulated knowledge of various projects developed over time.

### API Contracts and Documentation 

This template comes pre-configured with Swagger UI for API documentation. It maps all possible status code that may be returned from each endpoint, along with the required input payloads and the corresponding response objects.

### Docker

Docker swarm, docker secrets, docker compose.

### Middlewares, filters and Exception/Error handling

FluentValidation is used mainly on the RequestBodyValidationFilter class, and it's used to validate user input data, which are mainly represented as DTOs. This filter was created for substituing FluentValidation.AspNetCore automatic validation, since FluentValidation itself [does not recommend using it](https://docs.fluentvalidation.net/en/latest/aspnet.html). The filter adds async support for automatic validation.

Write about exception handling.

Write about RateLimiting.

### Authentication and Authorization

For authentication, we leverage from JwtBearer lib to use JWT tokens. One can check the implementation on the _Infrastructure_ project. The lib already implements all we need for handling authentication, and we provide the configurations properly (check the Auth/JwtSetup folder).

For authorization, we use a rich [Policy-based](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0) approach:

* We first created a PermissionRequirement, which has a string property called Permission. 
* Created a PermissionAttribute that expects a string (the policy name).
* Our handler is of generic type _PermissionRequirement_. Since we use JWT, it basically checks if the token has a Claims for the policies, and checks it has the required policy name.
* The last step is creating a PolicyProvider. Since we don't want to add all the policies in the startup (we already keep them in memory using the Policies class), a more elegant way is to create a PolicyProvider which will provide as required.

For better illustration, when we get a request:
* Controller checks if a specific policy is required and calls the PolicyProvider to get the policy.
* PolicyProvider return the Policy required, creating the PermissionRequirement.
* Handler checks if token has the required policy, represented by the PermissionRequirement.
* Handler sets the context as succeded or not.

The policies on this application rely on Constants that are also used on the migrations for automatically adding new Roles and Policies into the database. 

### Database and EFCore

This template uses Postgres alongside EFCore and it comes configured with Migrations.

Entity framework is configured to use _Eager loading_ and _no-tracking_.

Cancellation tokens are also passed around every request and handled properly.

Password storage is used using BCrypt algorithm with hash + salt.

### Logging

Log configuration can be set on appsettings.{ENVIRONMENT}.json. Check a table for the [Microsoft logs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-7.0#aspnet-core-and-ef-core-categories).

### Network

According to [Microsoft](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-7.0&tabs=visual-studio%2Clinux-ubuntu), APIs should either:
* Not listen on HTTP
* Close the connection with status code 400 (Bad Request) and not serve the request.

Therefore, we set our application to not listen on HTTP. One can check it in three different places: _launchSettings.json_, _Dockerfile_ and on _docker compose_. We did it reading [this](https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/) article.


### Testing

xUnit, NSubstitute, AutoFixture, TestContainers

## How to

[Add how to use this template.]