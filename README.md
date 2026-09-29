# StudentHub

A .NET 10 Web API that shows two data-access approaches side by side:

| Module  | Approach       | Tech                                              |
|---------|----------------|---------------------------------------------------|
| Student | Database-first | SQL table + stored procedure `sp_StudentDetail`, Dapper |
| Country | Code-first     | EF Core migrations, one generic repository for all masters |

Auth is JWT Bearer. Get a token from `POST /api/Auth/token`, then click **Authorize** in Swagger.

## Solution layout

```
StudentHub.slnx
src/
  Core/            cross-cutting extensions (middleware, compression, rate limit)
  Data/            entities, DTOs, ServiceResult, pagination models
  Repositories/    StudentRepository (Dapper), GenericRepository<T>, DbContexts, EF configurations
  Services/        business logic
  StudentHub/      API project (API.csproj): controllers, validators, Program.cs
tests/
  StudentHub.Tests/  xUnit tests
Database/
  StudentDetail.sql  table + stored procedure (SQL Server 2014+)
  Country.sql        optional script version of the Country table
```

## Prerequisites

- .NET 10 SDK and Visual Studio 2026 (or VS Code with C# Dev Kit)
- SQL Server 2014 or later
- Node 20+ if you want to run the React UI

## First run after cloning

### 1. Set the startup project

In Visual Studio, right-click **API** (`src/StudentHub`), then choose **Set as Startup Project**.

### 2. Add your secrets (not stored in git)

`appsettings.json` keeps the secret values empty on purpose. Put the real values in User Secrets.

**Visual Studio:** right-click **API**, choose **Manage User Secrets**, paste the JSON below and fill it in:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=StudentHubDB;User Id=sa;Password=YOUR_PASSWORD;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=True",
    "StudentHubDb": "<same connection string, plain or base64>"
  },
  "Jwt": {
    "SecretKey": "<any random string of at least 32 characters>"
  }
}
```

**Command line** (run from `src/StudentHub`):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=StudentHubDB;..."
dotnet user-secrets set "ConnectionStrings:StudentHubDb" "Server=...;Database=StudentHubDB;..."
dotnet user-secrets set "Jwt:SecretKey" "<32+ character random string>"
```

The secrets are saved to `%APPDATA%\Microsoft\UserSecrets\90a25a0e-7354-4a2f-9eb0-fbd4b8e1b39a\secrets.json`.

About the connection strings:

- `DefaultConnection` is used by EF Core (Country and Identity).
- `StudentHubDb` is used by Dapper (Student). You can give it the plain connection string or a base64 copy of it. To create the base64 copy:

  ```powershell
  [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes("Server=...;Database=StudentHubDB;..."))
  ```

User Secrets are only loaded when `ASPNETCORE_ENVIRONMENT=Development`. `launchSettings.json` already sets this. On a server, use environment variables instead, for example `ConnectionStrings__DefaultConnection` and `Jwt__SecretKey`.

### 3. Create the database

1. Create an empty database called `StudentHubDB`.
2. Run `Database/StudentDetail.sql` against it. This creates the table and stored procedure for Student.
3. Apply the EF Core migrations for Country and Identity. In the **Package Manager Console**:

   ```powershell
   Add-Migration InitialCreate -Project Repositories -StartupProject API -Context ApplicationDbContext
   Update-Database            -Project Repositories -StartupProject API -Context ApplicationDbContext
   ```

   Or with the CLI, from the repository root:

   ```powershell
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate -p src/Repositories -s src/StudentHub -c ApplicationDbContext
   dotnet ef database update            -p src/Repositories -s src/StudentHub -c ApplicationDbContext
   ```

   Skip `Add-Migration` if the `Migrations` folder already exists in the repository.

### 4. Trust the dev HTTPS certificate (once per machine)

```powershell
dotnet dev-certs https --trust
```

### 5. Run

Press **F5**. Swagger opens at `https://localhost:7096/swagger`.

1. Call `POST /api/Auth/token` to get a token. Every user is Admin.
2. Click **Authorize** and paste the token.
3. Try the `/api/Student` and `/api/Country` endpoints. Country supports paging with `?pageNumber=1&pageSize=5`.

## Tests

```powershell
dotnet test
```

## React UI (optional)

The UI lives in a separate project (`react-user-detail`). Its Vite dev server forwards `/api` to `https://localhost:7096`, so start the API first:

```powershell
npm install
npm run dev
```

Then open http://localhost:5173.

## Troubleshooting

| Error | Fix |
|-------|-----|
| `Connection string 'ConnectionStrings:StudentHubDb' is missing` | Add it to User Secrets (step 2). |
| `Jwt:SecretKey is missing or shorter than 32 bytes` | Add a key of 32+ characters to User Secrets. |
| `The ConnectionString property has not been initialized` | `DefaultConnection` is empty. Add it to User Secrets. |
| `Invalid object name 'dbo.Country'` | Run `Update-Database` (step 3). |
| `Could not find stored procedure 'sp_StudentDetail'` | Run `Database/StudentDetail.sql`. |
| HTTPS or certificate errors | Run `dotnet dev-certs https --trust`. |
