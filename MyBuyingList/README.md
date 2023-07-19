# My Buying List

This is a side project I created for practing .Net. It's inspired on Jason's clean architecture, but I'm adjusting how it's built doing some research and changing things based on my conclusions and preferences.

## Topics I've researched and conclusions
1. AutoMapper: DTOs, profile and testing



2. xUnit vs nUnit.
After some research over NUnit and xUnit, I decided to go with the second. The main reason for it is that bu default xUnit tests run 100% independently by default. That means, each test has it own instance, and because of that you can run it in parallel with no worries.
Another good indicator is that Microsoft itself started using xUnit on it's own projects.

3. Exceptions vs Result monad.
After some research, I got into the conclusion that I would keep Exceptions over Result monad. Some reasons for it:
* No need for third party libs. Always try to keep with Microsoft default techonologies. Of course one can build it's own Result type, but it would not be as powerfull as the others offered in others (eg https://github.com/louthy/language-ext)
* C# is a technologie that uses Exceptions. At some point, anyways, you would need to work with it, since the System libs work like that, so you would need to go to documentation anyways to check for exceptions types.
* Boilerplate code: tried to applied Result monad in this project. Got into the conclusion that for smalls projects, it may work fine. But for projects with many layers, keeping casting errors types can lead to boilerplate code. Exceptions were created exactly to avoid this type of coding. 
* One can create it's own Exception classes and handle them properly on the app's middlewares. 

4. FluentValidation: exceptions and testing.
According to Fluent Validation docs (https://docs.fluentvalidation.net/en/latest/advanced.html), we used it's recommended way to throw custom exceptions. That way we create a pattern for the app's middleware error handling. You can check it at Application/Common/Extensions.
One may ask about testing and how to mock it, since it's an extension method. Again, according to their own documentation, one should treat the lib as a black box, so no mocking is required. Doc link: https://docs.fluentvalidation.net/en/latest/testing.html

5. Authentication and authorization.
For this topic, we considered using Microsoft ASP.NET Identity Authentication or create our own mechanism. 
ASP.NET Identity is a feature-rich membership system by Microsoft for authentication and authorization in ASP.NET applications. It is feature rich (including user registration, login, password recovery, and account confirmation), supports database storage, offers flexible authentication options (windows and external providers, for example), but can be complex, and may impact performance in high scalability scenarios (because of DB).
We think Identity is very valuable and would probably be the least-effort path. But, since this is a learning project, we decided to implement our own mechanism. We chosed to do authentication with JWT using JwtBearer lib.
One important point to mention is where to store Jwt's token. After some research, decided to store in a cookie according to this article: https://medium.com/swlh/whats-the-secure-way-to-store-jwt-dd362f5b7914
For Authorization: ......

6. Database

Postgres because it's free and great. Code first because I have previous experience with database-first, but not with code first. Also, it seems that people are using it more. Seems easier for deploying and version control because of migrations. Also for testing and mocking.

7. Minimal API?

## TODOs
* Async
* Sessions, filters, more specific backend stuff in general for controllers.
* Event driven? Cancellation Token?
* Further knowledge on postgres and entity framework. Transactions and lazy loading
* Integration Testing
* Fix Docker
* AOT
* Frontend? React?
* Add refresh token & cookie with JWT
* Add hash password into database
* Add authorization with claims

## Others
* Health Checks
* Rate limit
* Security
* Benchmarks & Performance Tests