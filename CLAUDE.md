# 🌿 ALittleLeaf - Vibe Coding AI Instructions

## 🎯 Project Context

This is a refactoring project of "ALittleLeaf" – an e-commerce platform for plants and home decor.
Goal: Migrate from a monolithic ASP.NET Core MVC application to a decoupled architecture: React (Frontend) + .NET 9 Web API (Backend).

## 🛠 Tech Stack

### Frontend (`/Frontend`)

- **Framework:** React (Vite)
- **Styling/UI:** Bootstrap (Strictly use Bootstrap for BOTH Customer and Admin sides to prevent CSS conflicts).
- **Data Fetching & Server State:** TanStack Query (React Query) + Axios.
- **Client Global State:** Zustand.

### Backend (`/Backend`)

- **Framework:** .NET 9 Web API
- **Architecture:** Strict 3-Layer Architecture (Controllers -> Services -> Repositories).
- **Database:** SQL Server (Dockerized) + Entity Framework Core.
- **Authentication:** JWT (JSON Web Tokens).
- **Media Storage:** Cloudinary.

---

## 📐 AI Vibe Coding Rules & Conventions

### 1. General Workflow

- **Atomic Execution:** Tackle one feature module at a time (e.g., Auth first, then Products, then Cart, then VNPAY). Do not scatter changes across multiple unrelated files.
- **Source of Truth (Business Logic):** Always refer to the legacy MVC codebase (`ALittleLeaf/`) for business logic rules.
- **Source of Truth (UI):** Always extract the DOM structure and Bootstrap class names **directly from the legacy `.cshtml` files** in `ALittleLeaf/Views/` and `ALittleLeaf/Views/Shared/` to build React components. Do NOT rely solely on screenshots — the `.cshtml` files are the authoritative pixel-perfect reference. Use the `/Design` folder screenshots as a visual supplement only.
  - **Shared layout components** (`Header.jsx`, `Footer.jsx`, `MainLayout.jsx`): extract from `ALittleLeaf/Views/Shared/_Header.cshtml`, `_Footer.cshtml`, `_Layout.cshtml`.
  - **Page-specific components**: find the matching `.cshtml` in the relevant `ALittleLeaf/Views/<Controller>/` subdirectory and convert its exact HTML structure and Bootstrap classes into JSX.
  - **CSS**: combine the `.cshtml` DOM structure with any corresponding legacy CSS files (from `ALittleLeaf/wwwroot/css/`) to achieve a pixel-perfect match.

### 2. Frontend Development Rules (React)

- **Bootstrap Only:** Use Bootstrap classes and components for layout and styling. Do not introduce Tailwind CSS or custom CSS unless absolutely necessary for specific overrides.
- **Component Design:** Break down complex UI from the `.cshtml` files into reusable, smaller React components. Preserve the exact element hierarchy and class names found in the legacy views.
- **API Integration:** Use Axios instances with interceptors to automatically attach the JWT token. Wrap all API calls in custom React Query hooks (e.g., `useProducts`, `useLogin`).
- **Global State:** Keep Zustand stores minimal. Only use it for truly global client states like `useCartStore` or `useAuthStore`.

### 3. Backend Development Rules (.NET 9 Web API)

- **Strict 3-Tier Enforcement:** - `Controllers`: Only handle HTTP requests/responses, route validation, and calling Services. NO business logic here.
  - `Services`: Contain all business rules, calculations, and external API calls (e.g., Cloudinary, VNPAY).
  - `Repositories`: Only handle database operations via EF Core.
- **Cloudinary Integration:**
  - Read Cloudinary credentials (`CloudName`, `ApiKey`, `ApiSecret`, `Secure=true`) strictly from `.env` or `appsettings.json`.
  - Refactor all image upload/update logic in the Services layer to push to Cloudinary and return the secure URL to save in the DB.
  - Assume the initial database seeding already contains valid Cloudinary URLs (referenced from `A_Little_Leaf_category_image.txt` and `A_Little_Leaf_product_image.txt`).
- **Security:** Ensure all endpoints are protected with `[Authorize]` attributes where necessary, parsing the JWT correctly.
- **Documentation:** Maintain clean XML comments for Swagger UI.

### 4. File Structure Discipline

- Ensure strict separation between the `/Frontend` and `/Backend` directories.
- When generating new files, explicitly state the relative path to ensure they are created in the correct workspace.

### 5. Infrastructure & Testing Rules

- **Docker/DevOps:** When refactoring, DO NOT break the existing Docker setup. You must update `docker-compose.yml` and `Dockerfile` to support the new two-container architecture (Vite Frontend container + .NET 9 Web API container).
- **Testing:** Ensure `xUnit` test files in `ALittleLeaf.Tests` are updated to reflect the new Web API endpoints and Services. Maintain the integrity of the CI/CD pipeline (`.github/workflows/ci.yml`).

### 6. Golden Architectural Rules (Non-Negotiable)

- **Strict 3-Tier:** Controllers → Services → Repositories. No business logic in controllers, no DB calls in services directly — always go through the Repository layer.
- **Cart:** DB-backed `Cart` and `CartItem` tables. `UserId` is ALWAYS extracted from the JWT token server-side (`ClaimTypes.NameIdentifier`). Never trust a userId from the request body.
- **Frontend state:** Bootstrap ONLY for styling. React Query for all server state. Zustand only for global client state (`useAuthStore`, `useCartStore`). No Tailwind, no other CSS frameworks.
- **Database is strictly Code-First (EF Core Migrations).** Legacy SQL scripts (`ALittleLeaf.sql`) are deprecated for schema creation. All schema changes MUST be done via `dotnet ef migrations add`. Never re-introduce Database-First patterns.
- **Applying migrations locally:** The user MUST use the `--connection` override to prevent DotNetEnv networking conflicts. Example:
  ```
  dotnet ef database update --project Backend/ALittleLeaf.Api --connection "Server=localhost,1433;Database=ALittleLeafDecor;User Id=sa;Password=P@ssw0rd123;TrustServerCertificate=True"
  ```

  **DO NOT** modify the connection string in `appsettings.Development.json` and **DO NOT** configure LocalDB.

---

## 🚀 Progress Tracker

> **Rule:** Update this checklist at the end of every successfully completed phase.

- [X] Phase 0 — Workspace & Repository Preparation
- [X] Phase 1 — Backend Foundation (Web API project setup)
- [X] Phase 2 — Database & Cloudinary Integration
- [X] Phase 3 — Backend: Auth API
- [X] Phase 4 — Backend: Product & Category API
- [X] Phase 5 — Backend: Cart API
- [X] Phase 6 — Backend: Order & Checkout API
- [X] Phase 7 — Backend: Admin API
- [X] Phase 8 — Docker & CI/CD Update
- [X] Phase 9 — Frontend Foundation
- [X] Phase 10 — Frontend: Auth UI
- [X] Phase 11 — Frontend: Customer-Facing Pages (all pages complete including PaymentResultPage)
- [X] Phase 12 — Frontend: Admin Dashboard UI (12.1 ✓ AdminLayout/Sidebar/Header/AdminRoute; 12.2 ✓ AdminDashboardPage KPI+charts+low-stock; 12.3 ✓ AdminProductsPage list+delete + AdminProductFormPage create/edit+Cloudinary upload; 12.4 ✓ AdminOrdersPage list+filters + AdminOrderDetailPage detail+status update; 12.5 ✓ AdminUsersPage list+filters+sort + AdminUserFormPage edit+toggle-active; ✓12.6)
- [X] Phase 13 — Test Suite Migration (13.1 ✓ ALittleLeaf.Tests.csproj references Backend project; 13.2 ✓ all 13 xUnit test files compile and pass 0 errors; 13.3 ✓ FunctionalTests base URLs updated to localhost:3000 SPA / localhost:8081/api; 13.4 ✓ Selenium selectors refactored to match React DOM; 13.5/13.6 E2E suite skipped — unit tests 95/95 green)
- [X] Phase 14 — Cutover & Legacy Cleanup ← **ACTIVE** (14.2 ✓ webapp removed from docker-compose; 14.3 ✓ legacy ALittleLeaf/ MVC removed from sln; 14.4 ✓ Dockerfile.test removed; 14.5 ✓ CI finalized; 14.6 ✓ README updated)
