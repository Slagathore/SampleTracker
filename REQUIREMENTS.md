# SampleTracker — Requirements

All tools and dependencies needed to build and run this project locally.

---

## System Prerequisites

Install all of the following before cloning. Verify each one with the listed command.

| Tool                      | Minimum Version        | Verify             | Download                                                      |
| ------------------------- | ---------------------- | ------------------ | ------------------------------------------------------------- |
| .NET SDK                  | 10.x                   | `dotnet --version` | https://dotnet.microsoft.com/download                         |
| Node.js                   | 18 LTS or later        | `node --version`   | https://nodejs.org                                            |
| npm                       | 9+ (bundled with Node) | `npm --version`    | —                                                             |
| Git                       | Any                    | `git --version`    | https://git-scm.com                                           |
| Azure CLI _(deploy only)_ | Latest                 | `az --version`     | https://learn.microsoft.com/en-us/cli/azure/install-azure-cli |

> **Windows only:** `dev.bat` requires `cmd.exe` (standard on all Windows installs).

---

## Backend — NuGet Packages

Managed by `backend/SampleTracker.API/SampleTracker.API.csproj`. Restored automatically on `dotnet run` or `dotnet build`.

| Package                                         | Version | Purpose                          |
| ----------------------------------------------- | ------- | -------------------------------- |
| `Microsoft.EntityFrameworkCore.Sqlite`          | 10.0.6  | SQLite database provider         |
| `Microsoft.EntityFrameworkCore.SqlServer`       | 10.0.6  | SQL Server provider (production) |
| `Microsoft.EntityFrameworkCore.Design`          | 10.0.6  | EF Core migration tooling        |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.6  | JWT Bearer middleware            |
| `BCrypt.Net-Next`                               | 4.1.0   | Password hashing                 |
| `Swashbuckle.AspNetCore`                        | 6.9.0   | Swagger / OpenAPI UI             |
| `Microsoft.OpenApi`                             | 1.6.14  | OpenAPI model support            |
| `MediatR`                                       | 14.1.0  | In-process command/query bus     |
| `SQLitePCLRaw.core`                             | 3.0.2   | Low-level SQLite bindings        |

To restore manually:

```bash
cd backend/SampleTracker.API
dotnet restore
```

---

## Frontend — npm Packages

Managed by `frontend/sample-tracker-ui/package.json`. Install with `npm install`.

### Runtime dependencies

| Package                    | Version | Purpose                                  |
| -------------------------- | ------- | ---------------------------------------- |
| `react`                    | ^19.2.4 | UI framework                             |
| `react-dom`                | ^19.2.4 | React DOM renderer                       |
| `react-router-dom`         | ^7.14.1 | Client-side routing                      |
| `axios`                    | ^1.15.0 | HTTP client                              |
| `zustand`                  | ^5.0.12 | Lightweight global state                 |
| `recharts`                 | ^3.8.1  | Charts (dashboard stats)                 |
| `@radix-ui/react-dialog`   | ^1.1.15 | Accessible modal dialogs                 |
| `@radix-ui/react-label`    | ^2.1.8  | Accessible form labels                   |
| `@radix-ui/react-select`   | ^2.2.6  | Accessible select dropdowns              |
| `lucide-react`             | ^1.8.0  | Icon library                             |
| `class-variance-authority` | ^0.7.1  | Typed Tailwind variant helpers           |
| `clsx`                     | ^2.1.1  | Conditional class name utility           |
| `tailwind-merge`           | ^3.5.0  | Merge Tailwind classes without conflicts |

### Dev dependencies

| Package                       | Version  | Purpose                                 |
| ----------------------------- | -------- | --------------------------------------- |
| `vite`                        | ^8.0.4   | Build tool and dev server               |
| `@vitejs/plugin-react`        | ^6.0.1   | React Fast Refresh for Vite             |
| `tailwindcss`                 | ^4.2.2   | Utility-first CSS framework             |
| `@tailwindcss/vite`           | ^4.2.2   | Tailwind Vite plugin                    |
| `autoprefixer`                | ^10.5.0  | PostCSS vendor prefixing                |
| `postcss`                     | ^8.5.10  | CSS transformation pipeline             |
| `typescript`                  | ~6.0.2   | TypeScript compiler                     |
| `eslint`                      | ^9.39.4  | Linter                                  |
| `eslint-plugin-react-hooks`   | ^7.0.1   | React hooks lint rules                  |
| `eslint-plugin-react-refresh` | ^0.5.2   | React Refresh lint rules                |
| `typescript-eslint`           | ^8.58.0  | TypeScript ESLint integration           |
| `@types/react`                | ^19.2.14 | React type definitions                  |
| `@types/react-dom`            | ^19.2.3  | ReactDOM type definitions               |
| `@types/react-router-dom`     | ^5.3.3   | React Router type definitions           |
| `@types/node`                 | ^24.12.2 | Node.js type definitions                |
| `globals`                     | ^17.4.0  | Global variable declarations for ESLint |

To install:

```bash
cd frontend/sample-tracker-ui
npm install
```

---

## Recommended VS Code Extensions

Open the Extensions panel (`Ctrl+Shift+X`) and search for each ID:

| Extension                 | Publisher        | Purpose                                      |
| ------------------------- | ---------------- | -------------------------------------------- |
| C# Dev Kit                | Microsoft        | C# language support, IntelliSense, debugging |
| ESLint                    | Microsoft        | JavaScript/TypeScript linting                |
| Prettier                  | Prettier         | Code formatting                              |
| Tailwind CSS IntelliSense | Tailwind Labs    | Tailwind class autocomplete                  |
| REST Client               | Huachao Mao      | Test `.http` files directly in VS Code       |
| SQLite Viewer             | Florian Klampfer | Browse the local SQLite database             |
| Azure App Service         | Microsoft        | Deploy and manage Azure App Service          |
| GitLens                   | GitKraken        | Enhanced Git history and blame               |

---

## Runtime Configuration

The backend reads configuration from `appsettings.json` and (locally) `appsettings.Development.json`.
**Never commit secrets.** Use environment variables or Azure Key Vault in production.

| Key                         | Required | Description                                                               |
| --------------------------- | -------- | ------------------------------------------------------------------------- |
| `ConnectionStrings:Default` | Yes      | SQLite path (local) or SQL Server connection string (prod)                |
| `Jwt:Key`                   | Yes      | Signing secret — minimum 32 characters                                    |
| `Jwt:Issuer`                | Yes      | Token issuer claim (e.g. `SampleTracker`)                                 |
| `Jwt:Audience`              | Yes      | Token audience claim (e.g. `SampleTrackerUsers`)                          |
| `KeyVault:Uri`              | No       | Azure Key Vault URI; enables secret fetch at startup via Managed Identity |
