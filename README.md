# 🌿 ALittleLeaf — E-commerce Platform

[![CI](https://github.com/hoanhngjp/ALittleLeaf/actions/workflows/ci.yml/badge.svg)](https://github.com/hoanhngjp/ALittleLeaf/actions)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoftsqlserver)
![xUnit](https://img.shields.io/badge/Tests-20%2F20_passing-brightgreen?logo=checkmarx)

---

## 📖 Overview

**ALittleLeaf** is a production-grade, full-stack e-commerce platform for plants and home décor. Built as a decoupled **React 18 SPA + .NET 9 Web API**, the project was fully migrated from a legacy monolithic ASP.NET Core MVC application and has since been extended with three major production-ready architectural upgrades:

- 🚚 **Real-time logistics** via Giao Hàng Nhanh (GHN) — live shipping fees, automated order fulfilment, and webhook-driven tracking
- 🔐 **OAuth 2.0 / Google SSO** — secure single sign-on with account-merge logic and an explicit Anti-Account-Takeover (ATO) security gate
- ⚡ **Inventory Concurrency Control** — EF Core optimistic concurrency (`RowVersion`) prevents overselling under concurrent load; abandoned VNPay orders are automatically cancelled and stock restored by a native .NET `BackgroundService`

---

## ✨ Key Features & Architecture

### 🛒 E-commerce Core

- Full product catalogue with category filtering, full-text search, and pagination
- DB-backed shopping cart persisted per user via JWT identity (`UserId` extracted server-side from token — never trusted from request body)
- Checkout with **VNPay** (online payment with IPN server-to-server callback) and **COD** (Cash on Delivery)
- Idempotent payment confirmation — IPN and browser return URL both call the same handler; double-processing is structurally impossible

### 🚚 GHN Logistics Integration

- **Cascading Address Selection:** Province → District → Ward dropdowns populated from the GHN Master Data API with 24-hour server-side `IMemoryCache` — zero repeated external calls within a cache window
- **Dynamic Shipping Fee:** Calls `POST /v2/shipping-order/fee` with real address IDs, item weight, and insurance value; the live fee is reflected in the order total before the customer confirms
- **Automated Order Push:** After payment confirmation, a GHN shipping order is created automatically; the returned `GHN Order Code` is stored on the bill for end-to-end tracking
- **Webhook Receiver:** `POST /api/shipping/webhook` ingests real-time GHN status pushes and advances `ShippingStatus` on the bill independently of internal `OrderStatus`
- **Order Status Decoupling:** Two separate status axes prevent external shipping events from corrupting internal business state

| Field | Owner | Lifecycle |
|---|---|---|
| `OrderStatus` | ALittleLeaf | `PENDING → CONFIRMED → SHIPPING → COMPLETED / CANCELLED` |
| `ShippingStatus` | GHN (external) | Verbatim GHN states: `ready_to_pick`, `picked`, `delivering`, `delivered`, `damage`, `return`, … |

### 🔐 Security & Authentication

- **JWT Access Tokens** with refresh token rotation and per-token revocation stored in the database
- **Role-based access control** (`customer` / `admin`) enforced at the API layer via `[Authorize(Roles = "...")]`
- **Google OAuth 2.0 / SSO** — three-path account resolution on every Google login:
  1. **New user** — account created automatically with `AuthProvider = "google"` and a `null` password
  2. **Returning Google user** — re-authenticated silently
  3. **Existing local user (account merge)** — `GoogleId` and `AuthProvider` are linked; the original password hash is **preserved** so the user can still log in either way
- **Anti-Account-Takeover (ATO) gate** — `payload.EmailVerified` is explicitly checked before any account lookup or creation; an unverified Google email is rejected with `401 Unauthorized`, closing the vector where a malicious actor creates a Google account using someone else's email to hijack their local account

### ⚡ Inventory Concurrency & Background Automation

**Race Condition Prevention (Optimistic Concurrency)**

The classic e-commerce overselling problem — two concurrent requests reading the same stock value and both succeeding — is solved with an **EF Core `[Timestamp] RowVersion`** token on the `Product` entity. The flow:

1. Stock is **reserved at order creation** (`CreateOrderAsync`), not deferred to payment confirmation
2. `SaveChangesAsync` includes the `RowVersion` in the SQL `WHERE` clause automatically
3. If a concurrent transaction committed first, EF Core throws `DbUpdateConcurrencyException`, which the service catches and surfaces as a clean, user-facing Vietnamese error: *"Sản phẩm vừa hết hàng do có người khác vừa mua"*

**Automated VNPay Order Expiry (BackgroundService)**

When a user selects VNPay, an order is created and stock is reserved immediately. If the user closes the browser without paying, that inventory is held hostage indefinitely. `ExpiredOrderCancellationService` solves this:

- Runs as a native .NET `IHostedService` — **zero additional infrastructure**, no Hangfire or Redis required
- Scans every **5 minutes** for VNPay bills where `OrderStatus = PENDING` and `CreatedAtTime < now − 15 minutes`
- Sets `OrderStatus = CANCELLED`, `PaymentStatus = cancelled`, and **restores reserved stock** in a single atomic `SaveChangesAsync`
- Uses `IServiceScopeFactory` for correct scoped-dependency resolution inside a singleton-lifetime hosted service

> ⚠️ **Scale-out note:** The service is safe for single-instance deployments (the current Docker Compose setup). Multi-instance deployments behind a load balancer would require a distributed lock (e.g., Redis `SETNX`) or a Hangfire-based scheduler to prevent duplicate processing.

### 🔧 Admin Portal

- Dashboard with KPI cards, revenue charts, and low-stock alerts
- Order management with advanced filters (status, date range, keyword, shipping state)
- **One-click "Sync to GHN"** — resends confirmed orders to GHN if the automatic push failed; guarded so it is only available when `paymentStatus = paid` and `ghnOrderCode` is empty
- **GHN Webhook Simulator** — built-in admin UI tool to fire mock GHN webhook events locally; no `ngrok` or public URL required for end-to-end state-transition testing
- Product CRUD with **Cloudinary** image upload and management
- User management with role assignment and account activation toggle
- **Dynamic Banner CMS** — full-lifecycle management of homepage slider banners: upload (multipart `IFormFile`), reorder (inline `DisplayOrder`), toggle visibility, and delete (removes the asset from Cloudinary atomically). Strict server-side validation rejects files over 5 MB or outside `image/jpeg | image/png | image/webp`. Every uploaded URL is rewritten to include Cloudinary's `f_auto,q_auto` transformation for automatic format and quality optimisation at the CDN edge. The public `GET /api/banners` endpoint is capped at 5 records at the repository layer (`.Take(5)`) so the homepage payload is always bounded regardless of how many banners are active. The React slider falls back to a built-in static array if the API returns an empty list, ensuring the homepage hero section is never blank.

---

## 🏗️ System Architecture

```
Frontend/   — React 18 (Vite) SPA     — nginx, port 3000
Backend/    — .NET 9 Web API           — Kestrel, port 8081
db          — SQL Server 2022          — port 1433 (Docker-internal)
```

| Layer | Technology |
|---|---|
| SPA | React 18 + Vite, Bootstrap 5, TanStack Query, Zustand, Axios |
| API | .NET 9 Web API, EF Core 9 Code-First, JWT Bearer, Cloudinary SDK |
| Auth | ASP.NET Identity `PasswordHasher`, `Google.Apis.Auth`, JWT + Refresh Tokens |
| Database | SQL Server 2022 (Dockerised), schema managed via EF Core migrations |
| Logistics | GHN Sandbox/Production REST API (typed `HttpClient`, `IMemoryCache`) |
| Payments | VNPay (HMAC-SHA512 signature, IPN + Return URL callback) |
| Media | Cloudinary (secure upload, public URL stored in DB) |
| Background | .NET native `BackgroundService` (`IHostedService`) |
| Testing | xUnit, Moq, `RichardSzalay.MockHttp`, EF Core InMemory provider |

---

## 🧪 Test Suite

The project includes a comprehensive automated test suite covering all critical business paths, including edge cases introduced by the concurrency and SSO features.

### Running tests

```bash
# Primary test suite (Google SSO + Order Concurrency + GHN + Background Job)
dotnet test Backend/ALittleLeaf.Tests/ALittleLeaf.Tests.csproj

# Legacy CI suite (Auth service + Order service + Admin/Customer controllers)
dotnet test ALittleLeaf.Tests/ALittleLeaf.Tests.csproj
```

**Current status: 20/20 passing ✅**

### Coverage

| Test Class | Scenarios Covered |
|---|---|
| `AuthServiceTests` | Google SSO — unverified email rejection (ATO gate), new user creation, existing local user account merge with password preservation |
| `OrderServiceTests` | Stock deduction at order creation, `DbUpdateConcurrencyException` → friendly error message, COD/VNPay payment status, empty cart guard, total amount calculation |
| `ExpiredOrderCancellationServiceTests` | Expired VNPay orders cancelled + stock restored, fresh orders ignored (SaveChanges never called) |
| `GhnServiceTests` | Province/district/ward deserialization, sandbox junk-data filtering, 24-hour cache hit (exactly 1 HTTP call per window), null-data graceful handling |
| `AdminOrderControllerTests` | Paginated listing, get by ID (found / not found), status update (success / empty body / not found) |
| `CustomerAccountControllerTests` | Login (success / failure), Register (success / duplicate email), Logout |

### Testing Design Patterns

**Wrapper/Adapter Pattern for Static SDK Methods**

`GoogleJsonWebSignature.ValidateAsync` is a `static` method on the Google SDK — impossible to mock with Moq directly. The solution is a thin `IGoogleTokenValidator` interface with a `GoogleTokenValidator` production implementation that delegates to the SDK. Tests inject a `Mock<IGoogleTokenValidator>` to simulate verified and unverified payloads without making real network calls, achieving full isolation.

```
IGoogleTokenValidator  ←  Mock<IGoogleTokenValidator>  (tests)
       ↑
GoogleTokenValidator   →  GoogleJsonWebSignature.ValidateAsync  (production)
```

---

## 🔑 Environment Variables

Copy `.env.example` to `.env` and fill in all values before starting the containers.

```dotenv
# ── Database ────────────────────────────────────────────────────────────────
DB_CONNECTION_STRING=Server=sqlserver;Database=ALittleLeafDecor;User Id=sa;Password=YourStrong@Password123;
MSSQL_SA_PASSWORD=YourStrong@Password123

# ── JWT ─────────────────────────────────────────────────────────────────────
JWT_SECRET=your_jwt_signing_secret_minimum_32_characters

# ── Cloudinary ──────────────────────────────────────────────────────────────
CLOUDINARY_CLOUD_NAME=your_cloud_name
CLOUDINARY_API_KEY=your_api_key
CLOUDINARY_API_SECRET=your_api_secret
CLOUDINARY_SECURE=true

# ── VNPay ───────────────────────────────────────────────────────────────────
VNPAY_TMNCODE=your_vnpay_merchant_code
VNPAY_HASHSECRET=your_vnpay_hash_secret
VNPAY_URL=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
Vnpay__PaymentBackReturnUrl=http://localhost:3000/payment-result
VNPAY_IPN_URL=https://your-ngrok-id.ngrok-free.app/api/payment/vnpay-ipn

# ── GHN (Giao Hàng Nhanh) ───────────────────────────────────────────────────
GHN_API_KEY=your_ghn_api_key          # Token header for all GHN API calls
GHN_SHOP_ID=your_ghn_shop_id          # ShopId header for fee calc + order push
GHN_BASE_URL=https://dev-online-gateway.ghn.vn/shiip/public-api
# Production: https://online-gateway.ghn.vn/shiip/public-api

# ── Google OAuth 2.0 ────────────────────────────────────────────────────────
GOOGLE_CLIENT_ID=your_google_oauth_client_id
```

The corresponding `appsettings.json` sections (values injected from `.env` at runtime):

```json
{
  "Ghn": {
    "ApiKey": "",
    "ShopId": 0,
    "BaseUrl": "https://dev-online-gateway.ghn.vn/shiip/public-api"
  },
  "Google": {
    "ClientId": ""
  }
}
```

The frontend requires one additional variable in `Frontend/alittleleaf-frontend/.env`:

```dotenv
VITE_API_URL=http://localhost:8081
VITE_GOOGLE_CLIENT_ID=your_google_oauth_client_id
```

---

## 🚀 Local Setup & Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- .NET 9 SDK (for running migrations from the host)
- Git

### 1. Clone the repository

```bash
git clone https://github.com/hoanhngjp/ALittleLeaf.git
cd ALittleLeaf
```

### 2. Configure environment variables

```bash
cp .env.example .env
# Edit .env — fill in all secrets (DB, JWT, Cloudinary, VNPay, GHN, Google)
```

Create a Google OAuth 2.0 Client ID at [Google Cloud Console](https://console.cloud.google.com/) → **APIs & Services → Credentials**. Add `http://localhost:3000` and `http://localhost:5173` as authorised JavaScript origins.

### 3. Start all containers

```bash
docker compose up -d --build
```

| Service | URL |
|---|---|
| Frontend (React SPA) | http://localhost:3000 |
| Backend (.NET 9 API) | http://localhost:8081 |
| Swagger UI | http://localhost:8081/swagger |
| Database (SQL Server) | `localhost:1433` (Docker-internal) |

### 4. Apply EF Core migrations (first run only)

The API container applies pending migrations automatically on startup. To run manually from the host:

```bash
dotnet ef database update \
  --project Backend/ALittleLeaf.Api \
  --connection "Server=localhost,1433;Database=ALittleLeafDecor;User Id=sa;Password=<YOUR_SA_PASSWORD>;TrustServerCertificate=True"
```

> ⚠️ Always use the `--connection` override as shown. Do **not** modify `appsettings.Development.json`.

### 5. Stop and tear down

```bash
# Stop containers — data preserved
docker compose down

# Full reset — removes database volume
docker compose down -v
```

---

## 📁 Project Structure

```
ALittleLeaf/
├── Backend/
│   ├── ALittleLeaf.Api/
│   │   ├── Controllers/          HTTP endpoints (Auth, Products, Cart, Orders, Admin, Shipping, Payment)
│   │   ├── Services/
│   │   │   ├── Auth/             AuthService, IGoogleTokenValidator, GoogleTokenValidator
│   │   │   ├── Order/            OrderService (stock reservation + concurrency)
│   │   │   ├── Background/       ExpiredOrderCancellationService (IHostedService)
│   │   │   ├── Shipping/         GhnService (typed HttpClient + IMemoryCache)
│   │   │   └── ...               Cart, VNPay, Admin, Cloudinary
│   │   ├── Repositories/         EF Core data access (strict 3-layer)
│   │   ├── Models/               EF Core entities (RowVersion on Product)
│   │   ├── DTOs/                 Request/response shapes
│   │   ├── Options/              Strongly-typed config (GhnOptions, VnPayOptions)
│   │   └── Migrations/           EF Core migration history
│   └── ALittleLeaf.Tests/        Primary xUnit suite (20 tests)
├── Frontend/
│   └── alittleleaf-frontend/
│       ├── src/pages/            Route-level React pages
│       ├── src/components/       Reusable UI components
│       ├── src/hooks/            Custom TanStack Query hooks (useAuth, useGoogleLogin, …)
│       └── src/store/            Zustand global state (useAuthStore, useCartStore)
├── ALittleLeaf.Tests/            Legacy CI xUnit suite
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
| `test-api` | Runs the full xUnit test suite (both suites) |

**Required GitHub Secrets:** `MSSQL_SA_PASSWORD`, `JWT_SECRET`, `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`, `VNPAY_TMNCODE`, `VNPAY_HASHSECRET`, `GHN_API_KEY`, `GHN_SHOP_ID`, `GOOGLE_CLIENT_ID`

---

## 👤 Default Seeded Accounts

| Role | Email | Password |
|---|---|---|
| Admin | hoanhnghiep2704@gmail.com | 4L27hN04Aa@ |
| Customer | Thuong12@gmail.com | Test@123 |
