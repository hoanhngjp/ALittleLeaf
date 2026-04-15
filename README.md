# 🌿 ALittleLeaf — E-commerce Web Application

[![CI](https://github.com/hoanhngjp/ALittleLeaf/actions/workflows/ci.yml/badge.svg)](https://github.com/hoanhngjp/ALittleLeaf/actions)

ALittleLeaf là một nền tảng thương mại điện tử chuyên kinh doanh cây cảnh và đồ trang trí nội thất.  
Dự án được xây dựng trên nền tảng **.NET 9 (ASP.NET Core)** với kiến trúc 3 lớp chuẩn mực, tích hợp quy trình DevOps (Docker & CI/CD) và kiểm thử tự động toàn diện.

---

## 📑 Mục lục

- [Tính năng chính](#-tính-năng-chính)
- [Kiến trúc & Công nghệ](#-kiến-trúc--công-nghệ)
- [Cài đặt và Chạy](#-cài-đặt-và-chạy-sử-dụng-docker)
- [Hướng dẫn chạy Test](#-hướng-dẫn-chạy-test)
- [CI/CD](#%EF%B8%8F-cicd-github-actions)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)

---

## 🚀 Tính Năng Chính

### 👤 Khách hàng (User)

| Tính năng | Mô tả |
|---|---|
| 🔐 Authentication | Đăng ký, Đăng nhập, Quản lý hồ sơ |
| 🛍 Mua sắm | Xem danh sách sản phẩm, Tìm kiếm, Lọc theo danh mục |
| 🛒 Giỏ hàng | Thêm / Sửa / Xóa sản phẩm trong giỏ |
| 💳 Thanh toán | Tích hợp cổng thanh toán trực tuyến VNPay |

### 🛠 Quản trị viên (Admin)

| Tính năng | Mô tả |
|---|---|
| 📦 Quản lý sản phẩm | Thêm, Sửa, Xóa, Upload ảnh sản phẩm |
| 📋 Quản lý đơn hàng | Xem trạng thái đơn hàng, doanh thu |
| 🏭 Quản lý kho | Theo dõi số lượng tồn kho |

---

## 🏗 Kiến Trúc & Công Nghệ

Dự án áp dụng **Kiến trúc 3 Lớp (3-Layer Architecture)** kết hợp với **Repository Pattern** và **Dependency Injection (DI)** để đảm bảo tính tách biệt và dễ bảo trì.

| Layer | Công nghệ |
|---|---|
| Backend | ASP.NET Core 9.0 (MVC) |
| Database | SQL Server 2022 (Dockerized) |
| Frontend | Razor Views, Bootstrap, jQuery |
| Unit Test | xUnit |
| E2E Test | Selenium WebDriver (Edge) |
| DevOps | Docker, Docker Compose, GitHub Actions |

---

## 🐳 Cài đặt và Chạy (Sử dụng Docker)

Đây là cách nhanh nhất để khởi chạy dự án mà không cần cài đặt SQL Server hay .NET SDK trên máy.

### Yêu cầu

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (đã cài đặt và đang chạy)
- [Git](https://git-scm.com/)

### Khởi chạy hệ thống

Mở terminal tại thư mục gốc của dự án và chạy lần lượt các lệnh sau:

```bash
# 1. Clone repository
git clone https://github.com/hoanhngjp/ALittleLeaf.git
cd ALittleLeaf

# 2. Tạo file .env chứa mật khẩu DB (thay đổi nếu muốn)
echo "MSSQL_SA_PASSWORD=Password@12345" > .env

# 3. Build và khởi chạy Web App + Database
docker-compose up -d --build webapp
```

✅ Sau khi chạy xong, truy cập website tại: **<http://localhost:8080>**

### Dừng hệ thống

```bash
docker-compose down
```

---

## 🧪 Hướng dẫn chạy Test

Dự án bao gồm 2 bộ kiểm thử tự động:

### 1. Unit Tests (Chạy trong Docker)

Kiểm tra logic xử lý nội bộ (Services, Repositories).

```bash
# Lệnh này sẽ dựng môi trường test cô lập và chạy test
docker-compose --profile testing up --build unit-tests
```

### 2. E2E Tests (Selenium)

Kiểm tra luồng người dùng thật trên trình duyệt Edge.  
> ⚠️ **Yêu cầu:** Website phải đang chạy tại `localhost:8080` và máy đã cài **.NET 9 SDK**.

```bash
# 1. Đảm bảo Web App đang chạy
docker-compose up -d webapp

# 2. Chạy E2E test từ máy host
dotnet test ALittleLeaf.FunctionalTests
```

---

## ⚙️ CI/CD (GitHub Actions)

Dự án đã được thiết lập quy trình **Continuous Integration (CI)** tự động.  
Mỗi khi có code mới được đẩy lên nhánh `master`, GitHub Actions sẽ tự động:

- ☁️ Checkout Code
- 🐳 Dựng môi trường Docker (Web & SQL Server)
- 🗄️ Khởi tạo Database (chạy script SQL mẫu)
- ✅ Chạy Unit Tests

Trạng thái Build & Test có thể xem tại tab **[Actions](https://github.com/hoanhngjp/ALittleLeaf/actions)** trên GitHub.

---

## 📂 Cấu trúc thư mục

```text
ALittleLeaf/
├── .github/
│   └── workflows/               # Cấu hình GitHub Actions (CI/CD)
├── ALittleLeaf/                 # Source code chính (Web App)
│   ├── Controllers/             # Presentation Layer
│   ├── Services/                # Business Logic Layer
│   ├── Repositories/            # Data Access Layer
│   ├── Views/                   # UI (Razor Views)
│   ├── wwwroot/                 # Static files (CSS, JS, Images)
│   └── Dockerfile               # Cấu hình Docker cho Web App
├── ALittleLeaf.Tests/           # Unit Tests Project (xUnit)
├── ALittleLeaf.FunctionalTests/ # E2E Tests Project (Selenium)
├── docker-compose.yml           # Định nghĩa các Services (Web, DB, Test)
├── Dockerfile.test              # Cấu hình Docker dành riêng cho Unit Test
├── ALittleLeaf.sln              # Solution File
└── README.md                    # Tài liệu dự án
```

---

© 2025 ALittleLeaf Project.
