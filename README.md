# SampleTracker

A production-quality **Lab Sample & Compliance Tracker** built as a full-stack portfolio project.
Users log analytical samples, assign methods, track workflow status, and view a live dashboard.
Clean architecture, real patterns, deployed to Azure as a single unit.

---

## Features

- **JWT Authentication** — secure login with BCrypt password hashing; role-based access (Analyst / Admin)
- **Sample Management** — create, list, filter by status, and delete samples
- **Status Workflow** — six-stage pipeline: `Received → InPrep → InAnalysis → QCReview → Complete → Rejected`
- **Analytical Methods** — assign named methods to samples
- **Dashboard** — live stats chart powered by Recharts
- **CSV Export** — one-click export of all sample records
- **Swagger UI** — interactive API docs in development (`/swagger`)
- **Azure Key Vault integration** — optional; reads `JwtKey` secret via Managed Identity when deployed

---

## Tech Stack

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

React is built by Vite and output directly into `backend/SampleTracker.API/wwwroot/`.
The .NET host serves the SPA as static files, so there is **one origin, one deployment, and zero CORS config** — ever.

Internal business logic flows through a **MediatR command/query bus**.
Controllers are thin HTTP adapters. Handlers contain all the domain logic.

```
browser
  └─► .NET (Kestrel)
        ├─ GET /  →  wwwroot/index.html  (React SPA)
        └─ /api/* →  Controllers → MediatR → EF Core → SQLite/SQL Server
```

---

## Project Structure

```
SampleTracker/
├── dev.bat                         ← one-command local dev launcher
├── backend/
│   └── SampleTracker.API/
│       ├── Controllers/            ← thin HTTP adapters (no business logic)
│       │   ├── AuthController.cs
│       │   ├── MethodsController.cs
│       │   └── SamplesController.cs
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   └── Migrations/
│       ├── Features/               ← MediatR command/query handlers
│       │   └── Samples/
│       │       ├── CreateSample.cs
│       │       ├── ExportSamplesCsv.cs
│       │       ├── GetSamples.cs
│       │       ├── GetSampleStats.cs
│       │       └── UpdateStatus.cs
│       ├── Models/
│       │   ├── Sample.cs           ← SampleStatus enum lives here
│       │   ├── AnalyticalMethod.cs
│       │   ├── User.cs
│       │   └── DTOs/
│       ├── Services/
│       │   ├── TokenService.cs     ← JWT generation
│       │   └── KeyVaultService.cs  ← Azure Key Vault secret fetch
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── wwwroot/                ← Vite build output (served as SPA)
└── frontend/
    └── sample-tracker-ui/
        └── src/
            ├── api/client.ts       ← Axios instance + typed request helpers
            ├── components/
            │   ├── layout/         ← Sidebar, shell
            │   └── samples/        ← StatusBadge, sample-specific UI
            ├── hooks/useAuth.ts    ← Zustand auth store + helpers
            ├── pages/
            │   ├── Login.tsx
            │   ├── Dashboard.tsx
            │   └── Samples.tsx
            └── types/index.ts      ← shared TypeScript types
```

---

## Getting Started

### Prerequisites

See [REQUIREMENTS.md](./REQUIREMENTS.md) for full tool and version requirements.

### 1 — Clone

```bash
git clone https://github.com/Slagathore/SampleTracker.git
cd SampleTracker
```

### 2 — Configure the backend

`appsettings.Development.json` is already set up for local SQLite.
The only value you may want to change is the JWT key:

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

### 3 — Install frontend dependencies

```bash
cd frontend/sample-tracker-ui
npm install
```

### 4 — Run (quick launch)

From the `SampleTracker/` directory, double-click `dev.bat` or run:

```bat
dev.bat
```

This opens two terminal windows:

| Process         | URL                           |
| --------------- | ----------------------------- |
| .NET backend    | http://localhost:5294         |
| Swagger UI      | http://localhost:5294/swagger |
| Vite dev server | http://localhost:5173         |

### 5 — Log in

A guest account is seeded automatically on first run:

| Field    | Value                      |
| -------- | -------------------------- |
| Email    | `guest@sampletracker.demo` |
| Password | `guest`                    |

---

## Running Without dev.bat

**Backend:**

```bash
cd backend/SampleTracker.API
dotnet run --launch-profile http
```

**Frontend:**

```bash
cd frontend/sample-tracker-ui
npm run dev
```

**Build frontend into wwwroot (production-style):**

```bash
cd frontend/sample-tracker-ui
npm run build
# output lands in ../../backend/SampleTracker.API/wwwroot/
```

---

## API Reference

Full interactive docs are available at `/swagger` when running in Development mode.

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

## Azure Deployment

The app is designed to deploy as a single **Azure App Service** unit.

1. Build the frontend (`npm run build`) — Vite outputs into `wwwroot/`.
2. Publish the .NET project (`dotnet publish`).
3. Deploy the publish output to App Service.
4. Set App Service **Configuration** values:
   - `ConnectionStrings__Default` → your Azure SQL connection string
   - `Jwt__Key` → a strong secret (or set `KeyVault__Uri` and store `JwtKey` in Key Vault)
   - `Jwt__Issuer` → `SampleTracker`
   - `Jwt__Audience` → `SampleTrackerUsers`

Optional: enable a System-Assigned Managed Identity on the App Service and grant it `Key Vault Secrets User` on your Key Vault so secrets are pulled at startup automatically.

---

## License

MIT
