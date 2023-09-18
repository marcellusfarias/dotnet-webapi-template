# Shared Buying List

This is a side project I created for practing .Net. It's inspired on Jason's clean architecture, but I'm adjusting how it's built doing some research and changing things based on my conclusions and preferences.

## Topics I've researched and conclusions

### 1. DTOs and Mappings
This project uses DTOs objects to separate data that should be returned from the one stored in the database. I started using AutoMapper for doing the mappings, but I'm pretty convinced I'll remove it from the project, since it's hard to debug.

I'm using a custom attribute named AutoMapperMappingAttribute to initialize the mapper instance using Reflection. So, no need for Profiles and I don't need to keep a centralized place to put all mappings.

### 2. xUnit vs nUnit.
After some research over NUnit and xUnit, I decided to go with the first. The main reason for it is that xUnit tests run 100% independently by default. That means, each test has it own instance, and because of that one can boost parallelism.
Another good indicator is that Microsoft itself started using xUnit on it's own projects.

### 3. Exceptions vs Result monad.
After some research, I got into the conclusion that I would keep Exceptions over Result monad. Some reasons for it:
* No need for third party libs. Always try to keep with Microsoft default techonologies. Of course one can build it's own Result type, but it would take too much effort to be as powerfull as popular libs (eg https://github.com/louthy/language-ext)
* C# is a technologie that uses Exceptions. At some point, anyways, you would need to work with it, since the System libs work like that, so you would need to go to documentation anyways to check for exceptions types.
* Boilerplate code: tried to applied Result monad in this project. Got into the conclusion that for smalls projects, it may work fine. But for projects with many layers, keeping casting errors types can lead to boilerplate code. Exceptions were created exactly to avoid this type of coding. 
* One can create it's own Exception classes and handle them properly on the app's middlewares, which on my opinion is an elegant approach.

### 4. FluentValidation: usage, exceptions and testing.

FluentValidation is used mainly on the RequestBodyValidationFilter class, and it's used to validate user input data, which are mainly represented as DTOs. This filter was created for substituing FluentValidation.AspNetCore automatic validation, since FluentValidation itself does not recommend using it (check https://docs.fluentvalidation.net/en/latest/aspnet.html). The filter adds async support for automatic validation.

According to the docs (https://docs.fluentvalidation.net/en/latest/advanced.html), it's recommended to throw your own custom exceptions. We handle them on the app's ErrorHandlingMiddleware.

For testing: according to the docs (https://docs.fluentvalidation.net/en/latest/testing.html) one should not mock it, but treat it as a black box.

### 5. Authentication and authorization

For this topic, there was the option to use Microsoft ASP.NET Identity. 

> ASP.NET Identity is a feature-rich membership system by Microsoft for authentication and authorization in ASP.NET applications. It includes user registration, login, password recovery, account confirmation, supports database storage, offers flexible authentication options (windows and external providers), but can be complex and add unnecessary functionallity for the application.

Since the main goal for this application is learning, I chosed to implement my own authentication and authorization mechanisms. For authentication, we leverage from JwtBearer lib to use JWT tokens. One can check the implementation on the _Infrastructure_ project.

One important point to mention is where to store Jwt's token. After some research, decided to store in a cookie according to this article: https://medium.com/swlh/whats-the-secure-way-to-store-jwt-dd362f5b7914.

Current TODOs for authentication: store secret on a appropriated place (probably using docker secrets), create refreshing token mechanism and configure Cookie.

For Authorization, I created an elegant way that uses Attributes and overrides the PolicyProvider. It relies on Constants that are also used on the migrations for automatically adding new Roles and Policies into the database. The custom AuthorizationHandler checks if the required policy is attached on the JWT token.

### 6. Database

Chosed Postgres over MSSQL because it's free. Using a code-first because I already have previous experience with database-first and I prefer it when using DDD. Migration's support is great on EF Core and the deployment process is also facilitated since it runs the migrations automatically. 

### 7. EFCore

Before choosing EFCore, I did a research on Dapper. I see the performance difference currently is huge. On the other hand, EFCore offers tons of functionalities. One thing that made me stick with EFCore is that the performance is improved a lot on .Net 8 (closer to Dapper, but still not better. Check: https://www.youtube.com/watch?v=Q4LtKa_HTHU). But I plan to use Dapper at least a little on this application for learning purposes.

I'm using a lazy-load approach. 

TODO: 

Write about the following topics:
* Which loading data method chosed and why: https://learn.microsoft.com/en-us/ef/core/querying/related-data/
* Tracking vs no tracking

## TODOs

### Async support

Of course this application will have async support. Since I was testing many things in parallel, I let it for the future, but it's the next thing to be added. I will also add CancellationToken.

### Session and cache

As mentioned on "Authentication and authorization", Cookie implementation will be added (https://learn.microsoft.com/pt-br/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0). Need to create refresh token mechanism.

I also plan to add Cache for this. 

### Minimal API

Planning to move on to Minimal APIs.

### Github Actions

Configure Github actions pipeline.

### Integration Testing

Will add integration testing. I'm willing to use TestContainers.

### Docker

Complete Docker support on this application. Since I already previous experience on it, it's not my most urgent goal. I will probably stick with Docker Swarm, add Docker Secrets and configure resource limits.

### Frontend

Planning to add an UI that will be a separated project, since I want to keep the API and the Interface project's separated. Not convinced yet if I should choose Blazor over React.

### Security

Store the user passwords using hash function.
TODO: review security aspects learned on the SecureFlag platform and apply them on this application.

### Others

Other interesting topics that may come in the future:
* AOT: test and play with it, specially do a comparison on the Docker image footprint.
* Think about reusing validating rules in DTOs.
* Health Checks support.
* Rate limit support.
* Benchmarks & Performance Tests.
* THINK: Should put testing project inside app project so I can make the classes internal instead of public?
* Make the API documentation more robust. Using swagger.
* Add HTTPS
* Review ALL middlewares and configure as needed in the app.
* Add Constants for string lengths
* * Read about Event Driven design and decide if it's worth applying it in here.