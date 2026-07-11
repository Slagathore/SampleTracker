# SampleTracker

A lab sample and compliance tracker (a LIMS). You log analytical samples, assign
methods, push each one through a six stage QC workflow, and export records for audits.
The whole thing, backend and frontend, runs as a single Azure App Service.

### Live demo

**https://sampletrackerccc.azurewebsites.net/**

Click **"Login as guest"** to poke around the full system. No account needed.

> Heads up on first load: this runs on Azure's free tier, so the app sleeps after
> a while with no traffic. The first request after a sleep can take 30 to 60 seconds
> to wake the server. Give it a moment; it is quick after that.

---

## What it does

You log analytical samples, assign named methods, move each sample along a status
pipeline, and watch a live dashboard. One backend, one frontend, one deployment.

## What's in it

- Login with JWT and BCrypt password hashing, with two roles (Analyst and Admin)
- Create, list, filter by status, and delete samples
- A six stage status pipeline: `Received, InPrep, InAnalysis, QCReview, Complete, Rejected`
- Named analytical methods you assign to samples
- A dashboard with a live stats chart (Recharts)
- CSV export of all sample records
- Swagger UI for the API in development (`/swagger`)
- Azure Key Vault support: reads the `JwtKey` secret through Managed Identity when deployed

---

## Tech stack

| Layer       | Technology                                                |
| ----------- | --------------------------------------------------------- |
| Backend     | ASP.NET Core (.NET 10), MediatR, Entity Framework Core 10 |
| Database    | SQLite (local dev) / SQL Server (production)              |
| Auth        | JWT Bearer, BCrypt.Net                                    |
| API Docs    | Swashbuckle / OpenAPI                                     |
| Frontend    | React 19, TypeScript, Vite                                |
| UI          | Tailwind CSS v4, Radix UI, Lucide React                   |
| State       | Zustand                                                   |
| Charts      | Recharts                                                  |
| HTTP Client | Axios                                                     |
| Routing     | React Router v7                                           |

---

## Architecture

Vite builds React straight into `backend/SampleTracker.API/wwwroot/`. The .NET host
serves the SPA as static files, so there is one origin, one deployment, and no CORS
config to deal with.

Business logic runs through a MediatR command/query bus. Controllers are thin HTTP
adapters and the handlers hold the actual domain logic.

```
browser
  └─► .NET (Kestrel)
        ├─ GET /  →  wwwroot/index.html  (React SPA)
        └─ /api/* →  Controllers → MediatR → EF Core → SQLite/SQL Server
```

---

## Project structure

```
SampleTracker/
├── dev.bat                         local dev launcher
├── backend/
│   └── SampleTracker.API/
│       ├── Controllers/            thin HTTP adapters (no business logic)
│       │   ├── AuthController.cs
│       │   ├── MethodsController.cs
│       │   └── SamplesController.cs
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   └── Migrations/
│       ├── Features/               MediatR command/query handlers
│       │   └── Samples/
│       │       ├── CreateSample.cs
│       │       ├── ExportSamplesCsv.cs
│       │       ├── GetSamples.cs
│       │       ├── GetSampleStats.cs
│       │       └── UpdateStatus.cs
│       ├── Models/
│       │   ├── Sample.cs           SampleStatus enum lives here
│       │   ├── AnalyticalMethod.cs
│       │   ├── User.cs
│       │   └── DTOs/
│       ├── Services/
│       │   ├── TokenService.cs     JWT generation
│       │   └── KeyVaultService.cs  Azure Key Vault secret fetch
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── wwwroot/                Vite build output (served as the SPA)
└── frontend/
    └── sample-tracker-ui/
        └── src/
            ├── api/client.ts       Axios instance and typed request helpers
            ├── components/
            │   ├── layout/         Sidebar, shell
            │   └── samples/        StatusBadge, sample UI
            ├── hooks/useAuth.ts    Zustand auth store and helpers
            ├── pages/
            │   ├── Login.tsx
            │   ├── Dashboard.tsx
            │   └── Samples.tsx
            └── types/index.ts      shared TypeScript types
```

---

## Getting started

### Prerequisites

See [REQUIREMENTS.md](./REQUIREMENTS.md) for the full tool and version list.

### 1. Clone

```bash
git clone https://github.com/Slagathore/SampleTracker.git
cd SampleTracker
```

### 2. Configure the backend

`appsettings.Development.json` is already set for local SQLite. The one value you may
want to change is the JWT key:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=sampletracker.db"
  },
  "Jwt": {
    "Key": "CHANGE_THIS_TO_A_32_CHAR_RANDOM_STRING_LOCAL",
    "Issuer": "SampleTracker",
    "Audience": "SampleTrackerUsers"
  }
}
```

### 3. Install frontend dependencies

```bash
cd frontend/sample-tracker-ui
npm install
```

### 4. Run (quick launch)

From the `SampleTracker/` directory, double click `dev.bat` or run:

```bat
dev.bat
```

This opens two terminal windows:

| Process         | URL                           |
| --------------- | ----------------------------- |
| .NET backend    | http://localhost:5294         |
| Swagger UI      | http://localhost:5294/swagger |
| Vite dev server | http://localhost:5178         |

### 5. Log in

A guest account is seeded on first run:

| Field    | Value                      |
| -------- | -------------------------- |
| Email    | `guest@sampletracker.demo` |
| Password | `guest`                    |

---

## Running without dev.bat

Backend:

```bash
cd backend/SampleTracker.API
dotnet run --launch-profile http
```

Frontend:

```bash
cd frontend/sample-tracker-ui
npm run dev
```

Build the frontend into wwwroot (production style):

```bash
cd frontend/sample-tracker-ui
npm run build
# output lands in ../../backend/SampleTracker.API/wwwroot/
```

---

## API reference

Full interactive docs live at `/swagger` when running in Development mode.

| Method | Route                      | Auth   | Description                             |
| ------ | -------------------------- | ------ | --------------------------------------- |
| POST   | `/api/auth/login`          | None   | Returns a JWT                           |
| GET    | `/api/samples`             | Bearer | List all samples (filter by `?status=`) |
| POST   | `/api/samples`             | Bearer | Create a sample                         |
| PATCH  | `/api/samples/{id}/status` | Bearer | Advance sample status                   |
| DELETE | `/api/samples/{id}`        | Admin  | Delete a sample                         |
| GET    | `/api/samples/stats`       | Bearer | Dashboard stat counts                   |
| GET    | `/api/samples/export`      | Bearer | Download CSV                            |
| GET    | `/api/methods`             | Bearer | List analytical methods                 |

---

## Azure deployment

The app deploys as a single Azure App Service.

1. Build the frontend (`npm run build`). Vite outputs into `wwwroot/`.
2. Publish the .NET project (`dotnet publish`).
3. Deploy the publish output to App Service.
4. Set these App Service **Configuration** values:
   - `ConnectionStrings__Default` to your Azure SQL connection string
   - `Jwt__Key` to a strong secret (or set `KeyVault__Uri` and store `JwtKey` in Key Vault)
   - `Jwt__Issuer` to `SampleTracker`
   - `Jwt__Audience` to `SampleTrackerUsers`

Optional: turn on a system assigned Managed Identity for the App Service and give it
`Key Vault Secrets User` on your Key Vault, and secrets get pulled at startup.

---

## License

MIT
