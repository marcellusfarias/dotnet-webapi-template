### Create migration

dotnet ef migrations add MigrationName --project ../Infrastructure/Infrastructure.csproj --startup-project ../Web/Web.csproj --output-dir ../Infrastructure/Persistence/Migrations