# ALittleLeaf — E-commerce Web Application

[![CI](https://github.com/hoanhngjp/ALittleLeaf/actions/workflows/ci.yml/badge.svg)](https://github.com/hoanhngjp/ALittleLeaf/actions)

ALittleLeaf is an e-commerce platform for plants and home decor, built as a decoupled **React SPA + .NET 9 Web API** architecture.

---

## Architecture

```
Frontend/   — React (Vite) SPA — served by nginx on port 3000
Backend/    — .NET 9 Web API   — exposed on port 8081
db          — SQL Server 2022  — exposed on port 1433 (internal)
```

- **Frontend**: React + Vite, Bootstrap, TanStack Query, Zustand, Axios
- **Backend**: .NET 9 Web API, EF Core (Code-First), JWT auth, Cloudinary for images
- **Database**: SQL Server (Dockerized), schema managed via EF Core migrations

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

---

## Running the Application

### 1. Clone the repo

```bash
git clone https://github.com/hoanhngjp/ALittleLeaf.git
cd ALittleLeaf
```

### 2. Create the `.env` file

Copy the example and fill in your secrets:

```bash
cp .env.example .env
```

Required variables:

| Variable | Description |
|---|---|
| `MSSQL_SA_PASSWORD` | SQL Server SA password (min 8 chars, mixed case + symbol) |
| `JWT_SECRET` | JWT signing secret (min 32 chars) |
| `CLOUDINARY_CLOUD_NAME` | Cloudinary cloud name |
| `CLOUDINARY_API_KEY` | Cloudinary API key |
| `CLOUDINARY_API_SECRET` | Cloudinary API secret |
| `VNPAY_TMNCODE` | VNPay merchant code |
| `VNPAY_HASHSECRET` | VNPay hash secret |

### 3. Start all containers

```bash
docker compose up -d
```

This starts:
- `db` — SQL Server (waits until healthy before dependents start)
- `api` — .NET 9 Web API at `http://localhost:8081` (Swagger: `http://localhost:8081/swagger`)
- `frontend` — React SPA at `http://localhost:3000`

### 4. Apply database migrations (first run only)

```bash
dotnet ef database update \
  --project Backend/ALittleLeaf.Api \
  --connection "Server=localhost,1433;Database=ALittleLeafDecor;User Id=sa;Password=<YOUR_SA_PASSWORD>;TrustServerCertificate=True"
```

### 5. Stop containers

```bash
docker compose down
```

To also remove the database volume (full reset):

```bash
docker compose down -v
```

---

## Running Unit Tests

### Locally (requires .NET 9 SDK)

```bash
dotnet test ALittleLeaf.Tests/ALittleLeaf.Tests.csproj
```

### Via Docker

```bash
docker build -f Dockerfile.test.api -t alittleleaf-tests .
docker run --rm alittleleaf-tests
```

Or with the compose testing profile (requires a running `db` container):

```bash
docker compose --profile testing up --exit-code-from unit-tests unit-tests
```

---

## Project Structure

```
ALittleLeaf/
├── Backend/
│   └── ALittleLeaf.Api/        .NET 9 Web API (Controllers, Services, Repositories, Models)
├── Frontend/
│   └── alittleleaf-frontend/   React (Vite) SPA (pages, components, hooks, stores)
├── ALittleLeaf.Tests/          xUnit unit tests for Backend services and controllers
├── ALittleLeaf.FunctionalTests/ Selenium E2E tests against the running SPA
├── docker-compose.yml
├── Dockerfile.test.api         Docker image for running the unit test suite
└── .env.example
```

---

## CI/CD

GitHub Actions runs on every push/PR to `master`:

| Job | What it does |
|---|---|
| `build-api` | Builds the Backend Docker image |
| `build-frontend` | Builds the Frontend Docker image |
| `test-api` | Runs the xUnit unit test suite via `Dockerfile.test.api` |

Required GitHub Secrets: `MSSQL_SA_PASSWORD`, `JWT_SECRET`, `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`, `VNPAY_TMNCODE`, `VNPAY_HASHSECRET`.

---

## Default Accounts (seeded)

| Role | Email | Password |
|---|---|---|
| Admin | hoanhnghiep2704@gmail.com | 4L27hN04Aa@ |
| Customer | Thuong12@gmail.com | Test@123 |
