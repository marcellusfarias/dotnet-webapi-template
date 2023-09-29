# Shared Buying List

This is a side project I created for practing .Net. It's inspired on Jason's clean architecture, but I'm adjusting how it's built doing some research and changing things based on my conclusions and preferences.

## Topics I've researched and conclusions

### 1. DTOs and Mappings
This project uses DTOs objects to separate data that should be returned from the one stored in the database. I'm not using Automapper for two reasons: don't want to add a extra dependency for something that isn't really a ton of code and because it's hard to debug.

### 2. xUnit vs nUnit.
After some research over NUnit and xUnit, I decided to go with the second. The main reason for it is that xUnit tests run 100% independently by default. That means, each test has it own instance, and because of that one can boost parallelism.
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

For Authorization, I created an elegant way that uses Attributes and overrides the PolicyProvider. It relies on Constants that are also used on the migrations for automatically adding new Roles and Policies into the database. The custom AuthorizationHandler checks if the required policy is attached on the JWT token.

### 6. Database

Chosed Postgres over MSSQL because it's free. Using a code-first because I already have previous experience with database-first and I prefer it when using DDD. Migration's support is great on EF Core and the deployment process is also facilitated since it runs the migrations automatically. 

### 7. EFCore

Before choosing EFCore, I did a research on Dapper. I see the performance difference currently is huge. On the other hand, EFCore offers tons of functionalities. I sticked with EFCore for this one.

This project uses _Eager loading_. I believe _lazy loading_ is prone to causing performance issues, since it increases database round trips if not properly used. Also, it is prone to breaking single responsability principle, since one could perform queries in the Application project, for example. With _eager loading_ we have more control over both things.

_Tracking_ vs _no-tracking_: there is a great article about it: https://learn.microsoft.com/en-us/ef/core/querying/tracking. In a nutshell, _tracking_ persists changes to the entity in the SaveChanges method and can improve performance if an entity is already in the context (meaning one less trip to the database). _No-tracking_ doesn't keep state, so it uses less memory and is faster in general, being great for _readonly_ operations. Furthermore, there is still _No-Tracking with Identity Resolution_: https://macoratti.net/22/05/ef_asnoidresol1.htm. It's good for relationship "one-to-many" for memory optimization, since it keep the repeated entity in the context. The default on this project is to track; however, for readonly operations, we tend to use no-tracking and no-tracking with identity resolution.

### 8. Cancellation Token

Cancellation Token will be added on every controller endpoint that has an I/O operation. Since we use _scoped_ database contexts and repository pattern, it should not be a problem. For more information, see: https://stackoverflow.com/questions/50329618/should-i-always-add-cancellationtoken-to-my-controller-actions.

Note that on the ErrorHandlingMiddleware we added a few lines to abort the request if the operation was cancelled. The reason for it is that since the client closed the connection (and that's how the Cancellation is triggered) there is no reason to return an HttpReponse.

## TODOs

This list is orderned by priority.

### Session and cache

As mentioned on "Authentication and authorization", Cookie implementation will be added (https://learn.microsoft.com/pt-br/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0). Need to create refresh token mechanism. Create docker secret for JWT key.

I also plan to add Cache for this. 

### Others

* Add Https.
* Add Health checks
* Add rate limit
* Review API documentation.
* Review ALL middlewares and configure as needed in the app.

### Try-Catch

Research how to resue Try Catch blocks. Specially for repository classes.

### Logging

Do proper logging.

### Async support

Research where I can return IAsyncEnumerator/Enumerable instead of task. 

### Integration Testing

Will add integration testing. I'm willing to use TestContainers.

### Docker

Complete Docker support on this application. Since I already previous experience on it, it's not my most urgent goal. I will probably stick with Docker Swarm, add Docker Secrets and configure resource limits.

### EFCore

Do deeper research on EFCore features.
"In case of tracking queries, results of Filtered Include may be unexpected due to navigation fixup. All relevant entities that have been queried for previously and have been stored in the Change Tracker will be present in the results of Filtered Include query, even if they don't meet the requirements of the filter. Consider using NoTracking queries or re-create the DbContext when using Filtered Include in those situations." (https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager)

### Security

Store the user passwords using hash function.
TODO: review security aspects learned on the SecureFlag platform and apply them on this application.

### Minimal API

Planning to move on to Minimal APIs.

### Github Actions

Configure Github actions pipeline.

### Frontend

Planning to add an UI that will be a separated project, since I want to keep the API and the Interface project's separated. Not convinced yet if I should choose Blazor over React.

### Others

Other interesting topics that may come in the future:
* AOT: test and play with it, specially do a comparison on the Docker image footprint.
* Think about reusing validating rules in DTOs.
* Benchmarks & Performance Tests.
* THINK: Should put testing project inside app project so I can make the classes internal instead of public?
* Add Constants for string lengths
* Read about Event Driven design and decide if it's worth applying it in here.