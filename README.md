# 🌿 ALittleLeaf — E-commerce Platform

[![CI](https://github.com/hoanhngjp/ALittleLeaf/actions/workflows/ci.yml/badge.svg)](https://github.com/hoanhngjp/ALittleLeaf/actions)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)
![xUnit](https://img.shields.io/badge/Tests-xUnit-green)

---

## 📖 Overview

**ALittleLeaf** is a full-stack e-commerce platform for plants and home décor, built as a decoupled **React SPA + .NET 9 Web API** architecture. The project was migrated from a legacy monolithic ASP.NET Core MVC application and now features a production-ready logistics integration with **Giao Hàng Nhanh (GHN)** for real shipping fee calculation, cascading address management, and end-to-end order fulfilment.

---

## ✨ Key Features

### 🛒 Shopping & Checkout
- Full product catalogue with category filtering, search, and pagination
- DB-backed cart (persisted per user via JWT identity)
- Checkout with **VNPay** (online payment with IPN callback) and **COD** (Cash on Delivery)
- Shipping fee calculated in real time against the GHN API before order placement

### 🚚 GHN Logistics Integration
- **Cascading Address Selection:** Province → District → Ward dropdowns populated from GHN Master Data API with 24-hour server-side caching
- **Dynamic Shipping Fee:** Calls `POST /v2/shipping-order/fee` with address, weight, and insurance value; result reflected in the order total before the user confirms
- **Order Push to GHN:** After payment confirmation, a GHN shipping order is created automatically; the returned GHN Order Code is stored against the bill for tracking
- **Webhook Receiver:** `POST /api/shipping/webhook` ingests real-time GHN status updates and advances `ShippingStatus` on the bill

### 📦 Order Lifecycle
- **Internal `OrderStatus`:** `PENDING → CONFIRMED → SHIPPING → COMPLETED / CANCELLED` — owned by the ALittleLeaf platform
- **External `ShippingStatus`:** 20+ GHN-native states (e.g., `ready_to_pick`, `picked`, `delivering`, `delivered`, `damage`, `return`) — sourced verbatim from GHN webhooks and kept separate from business status

### 🔧 Admin Portal
- Dashboard with KPI cards, revenue charts, and low-stock alerts
- Order management with advanced filters (status, date range, keyword, shipping state)
- **One-click "Sync to GHN":** Resends confirmed orders to GHN if the automatic push failed, with guard rails (only available when `paymentStatus = paid` and `ghnOrderCode` is empty)
- **GHN Webhook Simulator:** Built-in admin UI tool to fire mock GHN webhook events locally — no need to expose `localhost` to the internet for end-to-end state transition testing
- Product CRUD with Cloudinary image upload
- User management with role assignment and account activation toggle

### 🔐 Security & Auth
- JWT access tokens + refresh token rotation with revocation
- Role-based access (`customer` / `admin`) enforced at the API layer via `[Authorize(Roles = "...")]`

### 🧪 Automated Testing
- xUnit unit tests for all service and controller layers (Moq for dependencies)
- `RichardSzalay.MockHttp` for deterministic `HttpClient` testing in `GhnServiceTests`
- In-memory SQLite database via `DbContextFactory` for integration-style service tests

---

## 🏗️ Architecture

```
Frontend/   — React (Vite) SPA — served by nginx on port 3000
Backend/    — .NET 9 Web API   — exposed on port 8081
db          — SQL Server 2022  — port 1433 (internal to Docker network)
```

| Layer | Technology |
|---|---|
| SPA | React 18 + Vite, Bootstrap, TanStack Query, Zustand, Axios |
| API | .NET 9 Web API, EF Core Code-First, JWT, Cloudinary |
| Database | SQL Server (Dockerized), schema via EF Core migrations |
| Logistics | GHN Sandbox/Production REST API |
| Payments | VNPay |
| Media | Cloudinary |

### Order Status Decoupling

A key design decision is the **strict separation** of two status axes on every `Bill`:

| Field | Owner | Purpose |
|---|---|---|
| `OrderStatus` | ALittleLeaf platform | Tracks the *business* lifecycle: `PENDING → CONFIRMED → SHIPPING → COMPLETED / CANCELLED`. Advanced manually by admins or programmatically on payment confirmation. |
| `ShippingStatus` | GHN (external) | Mirrors GHN's own status codes verbatim (e.g., `ready_to_pick`, `delivering`, `delivered`, `return`). Advanced automatically by the webhook receiver. |

This prevents GHN delivery events from incorrectly mutating internal business state, and vice versa — an admin can cancel an order (`OrderStatus = CANCELLED`) even if GHN has already attempted delivery.

---

## 🔑 Environment Variables

### `.env` (Docker / runtime)

Copy `.env.example` to `.env` and fill in all values:

```dotenv
# Database
MSSQL_SA_PASSWORD=YourStrong@Password123

# JWT
JWT_SECRET=your_jwt_signing_secret_min_32_chars

# Cloudinary
CLOUDINARY_CLOUD_NAME=your_cloud_name
CLOUDINARY_API_KEY=your_api_key
CLOUDINARY_API_SECRET=your_api_secret

# VNPay
VNPAY_TMNCODE=your_vnpay_merchant_code
VNPAY_HASHSECRET=your_vnpay_hash_secret

# GHN (Giao Hàng Nhanh)
GHN_API_KEY=your_ghn_api_key
CLIENT_ID=your_ghn_client_id
SHOP_ID=your_ghn_shop_id
GHN_BASE_URL=https://dev-online-gateway.ghn.vn/shiip/public-api
```

> **GHN Notes:**
> - `GHN_API_KEY` — the `Token` header value for all GHN API calls.
> - `CLIENT_ID` / `SHOP_ID` — required for fee calculation and order push endpoints (`ShopId` header).
> - Change `GHN_BASE_URL` to `https://online-gateway.ghn.vn/shiip/public-api` for production.

### `appsettings.json` (Backend)

The API reads GHN config from the `Ghn` section (values injected from `.env` at runtime):

```json
{
  "Ghn": {
    "ApiKey": "",
    "ShopId": 0,
    "BaseUrl": "https://dev-online-gateway.ghn.vn/shiip/public-api"
  }
}
```

---

## 🚀 Local Setup & Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

### 1. Clone the repository

```bash
git clone https://github.com/hoanhngjp/ALittleLeaf.git
cd ALittleLeaf
```

### 2. Configure environment variables

```bash
cp .env.example .env
# Edit .env and fill in all required secrets (see Environment Variables section above)
```

### 3. Start all containers

```bash
docker compose up -d --build
```

This starts three services:
| Service | URL |
|---|---|
| Frontend (React SPA) | http://localhost:3000 |
| Backend (.NET 9 API) | http://localhost:8081 |
| Swagger UI | http://localhost:8081/swagger |
| Database (SQL Server) | `localhost:1433` (internal) |

### 4. Apply EF Core migrations (first run only)

The `api` container runs migrations automatically on startup. To apply manually from the host:

```bash
dotnet ef database update \
  --project Backend/ALittleLeaf.Api \
  --connection "Server=localhost,1433;Database=ALittleLeafDecor;User Id=sa;Password=<YOUR_SA_PASSWORD>;TrustServerCertificate=True"
```

> ⚠️ Always use the `--connection` override as shown above. Do **not** modify `appsettings.Development.json`.

### 5. Stop and tear down

```bash
# Stop containers (data preserved)
docker compose down

# Full reset — removes database volume
docker compose down -v
```

---

## 🧪 Running Tests

### Unit Tests (requires .NET 9 SDK)

```bash
# Legacy CI test suite
dotnet test ALittleLeaf.Tests/ALittleLeaf.Tests.csproj

# GHN & Order integration test suite
dotnet test Backend/ALittleLeaf.Tests/ALittleLeaf.Tests.csproj
```

### Via Docker

```bash
docker build -f Dockerfile.test.api -t alittleleaf-tests .
docker run --rm alittleleaf-tests
```

Test coverage includes:

| Suite | What is tested |
|---|---|
| `AuthServiceTests` | Login (valid, wrong password, locked, email not found), Register, Logout (token revocation) |
| `OrderServiceTests` | COD order creation, VNPay order creation, empty cart guard, stock deduction on fulfilment, insufficient stock guard |
| `AdminOrderControllerTests` | Paginated listing, get by ID (found / not found), status update (success / empty body / not found) |
| `CustomerAccountControllerTests` | Login (success / failure), Register (success / duplicate email), Logout |
| `GhnServiceTests` | Province/district/ward deserialization, sandbox junk-data filtering, 24-hour cache (exactly 1 HTTP call per cache window), null-data graceful handling |

---

## 📁 Project Structure

```
ALittleLeaf/
├── Backend/
│   ├── ALittleLeaf.Api/
│   │   ├── Controllers/          HTTP endpoints (Auth, Products, Cart, Orders, Admin, Shipping)
│   │   ├── Services/             Business logic (Auth, Order, Cart, GHN, VNPay, Admin)
│   │   ├── Repositories/         EF Core data access
│   │   ├── Models/               EF Core entities
│   │   ├── DTOs/                 Request/response shapes (Auth, Order, Admin, Shipping)
│   │   ├── Options/              Strongly-typed config (GhnOptions, VnPayOptions, ...)
│   │   └── Migrations/           EF Core migration history
│   └── ALittleLeaf.Tests/        xUnit tests (GHN service + order service)
├── Frontend/
│   └── alittleleaf-frontend/
│       ├── src/pages/            Route-level React pages
│       ├── src/components/       Reusable UI components
│       ├── src/hooks/            Custom React Query hooks
│       └── src/stores/           Zustand global state (auth, cart)
├── ALittleLeaf.Tests/            Legacy CI xUnit suite (services + controllers)
├── docker-compose.yml
└── .env.example
```

---

## ⚙️ CI/CD

GitHub Actions runs on every push and pull request to `master`:

| Job | What it does |
|---|---|
| `build-api` | Builds the Backend Docker image |
| `build-frontend` | Builds the Frontend Docker image |
| `test-api` | Runs the full xUnit test suite |

**Required GitHub Secrets:** `MSSQL_SA_PASSWORD`, `JWT_SECRET`, `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`, `VNPAY_TMNCODE`, `VNPAY_HASHSECRET`, `GHN_API_KEY`, `CLIENT_ID`, `SHOP_ID`

---

## 👤 Default Seeded Accounts

| Role | Email | Password |
|---|---|---|
| Admin | hoanhnghiep2704@gmail.com | 4L27hN04Aa@ |
| Customer | Thuong12@gmail.com | Test@123 |
