# Dotnet WebAPI Template

The final goal for this project is to become a template for new projects. It's inspired on Jason's clean architecture, but I'm adjusting how it's built doing some research and changing things based on my conclusions and preferences.

Other than Asp.Net, we currently use Postgres, EFCore, Docker, Swagger, JWT, xUnit, TestContainers, FluentValidation.

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

FluentValidation is used mainly on the RequestBodyValidationFilter class, and it's used to validate user input data, which are mainly represented as DTOs. This filter was created for substituing FluentValidation.AspNetCore automatic validation, since FluentValidation itself [does not recommend using it](https://docs.fluentvalidation.net/en/latest/aspnet.html). The filter adds async support for automatic validation.

According to the [docs](https://docs.fluentvalidation.net/en/latest/advanced.html), it's recommended to throw your own custom exceptions. We handle them on the app's ErrorHandlingMiddleware.

For testing: according to the [docs](https://docs.fluentvalidation.net/en/latest/testing.html) one should not mock it, but treat it as a black box.

### 5. Authentication

Authentication is handled by the authentication service (_IAuthenticationService_), which is used by authentication middleware. The authentication service uses registered _authentication handlers_ to complete authentication-related actions. The registered authentication handlers and their configuration options are called "_schemes_" (bearer, cookie, etc). It's important to know that _Microsoft.AspNetCore.Authentication.JwtBearer_ already implements the whole scheme.

Authentication handlers return _AuthenticationTicket_ if authentication is successful, and return "no result" or "failure" if not.

For this topic, there was the option to use [Microsoft ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-7.0). 

> ASP.NET Core Identity is a feature-rich membership system by Microsoft for authentication and authorization in ASP.NET applications. It includes user registration, login, password recovery, MFA, account confirmation, supports database storage, offers flexible authentication options (windows and external providers), but can be complex and add unnecessary functionallity for the application.

This project is starting as an API, so there is no need for all of these features. Also, for learning purposes, I decided to not go in that direction. For authentication, we leverage from JwtBearer lib to use JWT tokens. One can check the implementation on the _Infrastructure_ project. The lib already implements all we need for handling authentication, and we provide the configurations properly (check the Auth/JwtSetup folder).

### 6. Authorization

There are two approaches for authorization: a simple [Role-based](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-7.0) and a rich [Policy-based](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0), which we use. Note that there is also support for Claims and Resource-based authorization in combination with the above mentioned.

The primary service is the _IAuthorizationService_. It requires an user, a resource and a list of requirements/name of the policy. 
* The user: represented by ClaimsPrincipal class, it has a list of claims it owns.
* The resource: [in our case](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0#access-mvc-request-context-in-handlers), it's a _HttpContext_. The use of the Resource property is framework-specific. 
* The requirements. It can be a list of _IAuthorizationRequirement_ or a _policy_ name, which represents a collection of authorization requirements and the scheme or schemes they are evaluated against.

The _IAuthorizationHandler_ is responsible for handling it, and setting in the _AuthorizationHandlerContext_ if it succeded or not. For applying policies in the controllers, we can create a custom _PermissionAttribute_. One can add more than one handler (not our case).

Normally, one register the handler and also the list of policies on the startup. We did a bit different:

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

### 7. Database

Chosed Postgres over MSSQL because it's free. Using a code-first because I already have previous experience with database-first and migrations facilitates life. Migration's support is great on EF Core and the deployment process is also facilitated since it runs the migrations automatically. 

### 8. EFCore

Before choosing EFCore, I did a research on Dapper. I see the performance difference currently is huge. On the other hand, EFCore offers tons of functionalities. I sticked with EFCore for this one.

For loading data, there are three possibilities: _Eager loading_, _Lazy loading_ and _Explicit loading_ This project uses _Eager loading_. I believe _lazy loading_ is prone to causing performance issues, since it increases database round trips if not properly used. Also, it is prone to breaking single responsability principle, since one could perform queries in the Application project, for example. With _eager loading_ we have more control over both things. Similarly, we are not using _explicit loading_ to keep all db operations in the _Infrastructure_ project. I'm not sure if there may be cases in the future where _explicit loading_ would be a good fit.

_Tracking_ vs _no-tracking_: this [Microsoft's article](https://learn.microsoft.com/en-us/ef/core/querying/tracking) is very informative. In a nutshell, _tracking_ persists changes to the entity in the SaveChanges method and can improve performance if an entity is already in the context (meaning one less trip to the database). _No-tracking_ doesn't keep state, so it uses less memory and is faster in general, being great for _readonly_ operations, but one need to manually set the entity as _modified_. Furthermore, there is still _No-Tracking with Identity Resolution_ ([check example](https://macoratti.net/22/05/ef_asnoidresol1.htm)). It's good for relationship "one-to-many" for memory optimization, since it keep the repeated entity in the context. The default on this project is to _not_ track: since the Tracking context is in the dbContext, and we are setting it to be _scoped_, it wouldn't make sense for having the changing tracker in most of the operations (even inserts/updates).

### 9. Cancellation Token

Cancellation Token will be added on every controller endpoint that has an I/O operation. Since we use _scoped_ database contexts and repository pattern, it should not be a problem if creating commands properly (i.e., wrap the commands on a single transaction; i.e., perform save changes only at the end of the request). For more information, see [this](https://stackoverflow.com/questions/50329618/should-i-always-add-cancellationtoken-to-my-controller-actions).

Note that on the ErrorHandlingMiddleware we added a few lines to abort the request if the operation was cancelled. The reason for it is that since the client closed the connection (and that's how the Cancellation is triggered) there is no reason to return an HttpReponse.

### 10. HTTPS

According to [Microsoft](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-7.0&tabs=visual-studio%2Clinux-ubuntu), APIs should either:
* Not listen on HTTP
* Close the connection with status code 400 (Bad Request) and not serve the request.

Therefore, we set our application to not listen on HTTP. One can check it in three different places: _launchSettings.json_, _Dockerfile_ and on _docker compose_. We did it reading [this](https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/) article.

We are using a _self-signed_ certificate for development. When moving into production, we plan to add a valid one.

When moving into a Web App, Microsoft recommends adding  HttpsRedirection middleware and Hsts middleware.

### 11. Middlewares

Middlewares are a useful way for handling requests and responses in .Net. Each component:
* Chooses whether to pass the request to the next component in the pipeline.
* Can perform work before and after the next component in the pipeline.

For adding middleware to the pipeline, one can use [Run](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.runextensions.run?view=aspnetcore-7.0), [Map](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.mapextensions.map?view=aspnetcore-7.0) and [Use](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.useextensions.use?view=aspnetcore-7.0) extensions, and one can pass an _in-line delegates_ or a _reusable class_ as a parameter. In a nutshell, Run and Use add a middleware to the pipeline. The difference is that Run adds a terminator middleware (i.e., it won't call any further middleware in the pipeline, short-circuiting it) while Use can call the next one. 

The _Map_ is used for _branching_ the pipeline. [Branching](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0#branch-the-middleware-pipeline) is useful if we want to run a different pipeline based on the request path. The methods used for it are Map and [MapWhen](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.mapwhenextensions.mapwhen?view=aspnetcore-7.0). One can also use [UseWhen](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.usewhenextensions.usewhen) for it. Unlike with MapWhen, this branch is rejoined to the main pipeline if it doesn't short-circuit or contain a terminal middleware

See an example of all methods [here](https://www.codeproject.com/Tips/1069790/Understand-Run-Use-Map-and-MapWhen-to-Hook-Middl-2).

In .Net 7, there are many built-in [middlewares](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0#built-in-middleware) available to use. In the following table, we list the most common middlewares and check if we use them:

| Middlewares						| Used	|  Commentary	|
|-----------------------------------|:-----:|---------------|
| Authentication					|  Yes	|	Using "Bearer" schema with Jwt.			|
| Authorization						|  Yes	| We use our custom [policy-based authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0) and our own [policy provider](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider?view=aspnetcore-7.0).
| [Cookie Policy](https://learn.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-7.0)						|  No	| Not using Cookies.  			|
| CORS								|  No	| Haven't identified the need for [Cross-Origin Resource Sharing](https://developer.mozilla.org/pt-BR/docs/Web/HTTP/CORS) on this API yet.|
| Hsts & Https Redirection			|  No	| Not being used. We simply don't listen on HTTP. |
| Routing							|  Yes	| Not customizing it. Adding after the ErrorHandlingMiddleware.  |
| Endpoint							|  Yes	| This middleware is automatically added at the end of the pipeline as a terminal middleware. It's role is to register the endpoints that are going to be matched by the Routing, and when the endpoint is invoked it also is responsible for executing the _filter_ pipeline. For registering endpoint, we use MapControllers. |
| Rate Limiter						|	Yes	| Added a simple FixedWindowLimiter rate limiter just for the authentication endpoint. The middleware must be added after UseRouting on this case. Currently no queuing, 1 request per 5 seconds. |
| Static Files						|  No	| Not using yet since we don't have static files. This middleware is used to short-circuit requests, and provides no Authorization checks (if wanted, check [this](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0)). |
| Session							|	No	| Not being used while just an API. | 


Here is a list of middlewares that we don't use because it's not required (at least not yet). Most of them are related to this project still being only an API and specifics on HTTP. Some of them are just not necessary or are not worth.
* Forwarded Headers: forwards proxied headers onto the current request.
* DeveloperExceptionPage
* Header propagation: propagates HTTP headers from the incoming request to the outgoing HTTP Client requests.
* Http Logging: logs HTTP Requests and Responses. 
* Http Method Override: Allows an incoming POST request to override the method.
* OWIN: interop with OWIN-based apps, servers, and middleware.
* Request Localization: provides localization support (usually for multilingual websites). 
* Single Page Application (SPA): handles all requests from this point in the middleware chain by returning the default page
* W3CLogging: generates server access logs in the W3C Extended Log File Format
* WebSockets: not using any library that needs it.

Planning to add:
*  [Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0)
* Output Caching & Response Caching 

Investigate further in the future:
* Diagnostics: several separate middlewares that provide a developer exception page, exception handling, status code pages, and the default web page for new apps.
* Response compression & decompression
* [Url Rewriting](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/url-rewriting?view=aspnetcore-7.0)

### 12. Logging

ASP.NET logging documentation [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-7.0) and [here](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line). There are four logging providers on the generic host: Console, Debug, EventSource and EventLog. We added only console for this API.

Log configuration can be set on appsettings.{ENVIRONMENT}.json. Check a table for the [Microsoft logs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-7.0#aspnet-core-and-ef-core-categories).

For debugging in the future, one may also want to use [HttpLogging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-7.0).

## Backlog

### Before releasing

This list is orderned by priority.

#### Tests

* Finish writing tests.
* How to [test](https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-7.0) middlewares. 
* On integration test, create a user for testing with admin rights separated from migrations.

#### Security

* When moving into production, must set a secure HTTPS certificate.
* TODO: review security aspects learned on the SecureFlag platform and apply them on this application.
* Remove sensitive information from logging

#### Docker & Hosting

* Complete Docker support on this application. 
* I will probably stick with Docker Swarm, add Docker Secrets and configure resource limits.
* Create docker secret for JWT key and HTTPs certs. .Net [Host](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication?view=aspnetcore-7.0).
* Research best way to configure which environment is running. Interesting [link](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0#determining-the-environment-at-runtime). Another [link](https://stackoverflow.com/questions/32548948/how-to-get-the-development-staging-production-hosting-environment-in-configurese).

### After releasing 

#### Caching

Output caching. Redis.

#### API Documentation

Add deeper level of details in the API documentation. Add sections like
* Authentication
* Rate Limits
* [ProblemDetails](https://code-maze.com/using-the-problemdetails-class-in-asp-net-core-web-api/) 

Read articles about how to create a great API documentation. [Example](https://swagger.io/blog/api-documentation/best-practices-in-api-documentation/).

#### EFCore

Do deeper research on EFCore features.
"In case of tracking queries, results of Filtered Include may be unexpected due to navigation fixup. All relevant entities that have been queried for previously and have been stored in the Change Tracker will be present in the results of Filtered Include query, even if they don't meet the requirements of the filter. Consider using NoTracking queries or re-create the DbContext when using Filtered Include in those situations." (https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager)

#### Github Actions

Configure Github actions pipeline.

#### Middleware

Finish reading middleware documentation.

* Read about [factory based middlewares](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/extensibility?view=aspnetcore-7.0) and [convention based](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-7.0) middleware.
* Read [this](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/request-response?view=aspnetcore-7.0).

#### Frontend

When migrating to WebApp:
* Choose between React/Angular with Typescript or Blazor.
* Implement related auth pieces. 

When doint it, need to implement further Authentication pieces. 
* As mentioned on "Authentication and authorization", [cookie implementation](https://learn.microsoft.com/pt-br/aspnet/core/undamentals/app-state?view=aspnetcore-7.0) will be added.
* Configure properly where to store the JWT token. Chosed options [here](https://medium.com/swlh/whats-the-secure-way-to-store-jwt-dd362f5b7914).
* Create refresh token mechanism (https://www.youtube.com/watch?v=HsypCNm56zs).  
* Cache for storing the refresh token. 
* Add MFA.
* Add http redirection.
* Read [ASP.NET Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-7.0&tabs=visual-studio)

Useful docs: 
* [React](https://learn.microsoft.com/en-us/aspnet/core/client-side/spa/react?view=aspnetcore-7.0&tabs=visual-studio)
* [Angular](https://learn.microsoft.com/en-us/aspnet/core/client-side/spa/angular?view=aspnetcore-7.0&tabs=visual-studio)

#### General (not priority orderned)

ASP.NET Interesting topics/can go deeper:

* Routing: [read](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0) further about it.
* Rate limiting: investigate if need more complex rate limiting. 
* Health checks.
* Minimal APIs
* Logging

Other topics:
* AOT: test and play with it, specially do a comparison on the Docker image footprint.
* Benchmarks & Performance Tests.
* Create logger properly on startup.
* THINK about reusing validating rules in DTOs.
* THINK: Should put testing project inside app project so I can make the classes internal instead of public?
* THINK: research how to resue (if possible?) Try Catch blocks. Specially for repository classes.
* THINK: Good idea in the future to set eventIDs when logging?
* StringLength attribute on Domain classes?
* Research how to change language of fluent validation.
* Research if it's possible to use data annotations for string length

Testing:
* Research how to make current integration tests faster.
* Research if there is any specific EFCore feature that should be tested.
* Research how to create E2E tests. Check if this [jwt tool](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-7.0&tabs=windows) is useful.