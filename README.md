# Project creation
Used Visual Studio for Mac and chose new solution -> .NET Core -> App -> React.js.

# Run
In solution root, run `dotnet restore`.
Navigate to ClientApp and run`npm install`.

Run project from Rider, should run with working server call on https://localhost:5001

# Set with db
## Add SQLite package
Navigate to project root and run `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`

## Once you have a model, you use migrations to create a database.
Run `dotnet ef migrations add InitialCreate` to scaffold a migration and create the initial set of tables for the model.
Run `dotnet ef database update` to apply the new migration to the database. This command creates the database before applying migrations.
