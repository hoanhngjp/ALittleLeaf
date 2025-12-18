🌿 ALittleLeaf - E-commerce Web Application

ALittleLeaf là một nền tảng thương mại điện tử chuyên kinh doanh cây cảnh và đồ trang trí nội thất. Dự án được xây dựng dựa trên nền tảng .NET 9 (ASP.NET Core) với kiến trúc 3 lớp chuẩn mực, tích hợp quy trình DevOps (Docker & CI/CD) và kiểm thử tự động toàn diện.

🚀 Tính Năng Chính
👤 Khách hàng (User)
Authentication: Đăng ký, Đăng nhập, Quản lý hồ sơ.

Mua sắm: Xem danh sách sản phẩm, Tìm kiếm, Lọc theo danh mục.

Giỏ hàng: Thêm/Sửa/Xóa sản phẩm trong giỏ.

Thanh toán: Tích hợp cổng thanh toán trực tuyến VNPay.

🛠 Quản trị viên (Admin)
Quản lý sản phẩm: Thêm, Sửa, Xóa, Upload ảnh sản phẩm.

Quản lý đơn hàng: Xem trạng thái đơn hàng, doanh thu.

Quản lý kho: Theo dõi số lượng tồn kho.

🏗 Kiến Trúc & Công Nghệ
Dự án áp dụng Kiến trúc 3 Lớp (3-Layer Architecture) kết hợp với Repository Pattern và Dependency Injection (DI) để đảm bảo tính tách biệt và dễ bảo trì.

Backend: ASP.NET Core 9.0 (MVC).

Database: SQL Server 2022 (Dockerized).

Frontend: Razor Views, Bootstrap, jQuery.

Testing:

Unit Test: xUnit.

E2E Test: Selenium WebDriver (Edge).

DevOps:

Docker & Docker Compose.

GitHub Actions (CI Pipeline).

🐳 Cài đặt và Chạy (Sử dụng Docker)
Đây là cách nhanh nhất để khởi chạy dự án mà không cần cài đặt SQL Server hay .NET SDK trên máy.

1. Yêu cầu
Docker Desktop (đã cài đặt và đang chạy).

Git.

2. Khởi chạy hệ thống
Mở terminal tại thư mục gốc của dự án:

Bash

# 1. Clone repository
git clone https://github.com/hoanhngjp/ALittleLeaf.git
cd ALittleLeaf

# 2. Tạo file .env (Chứa mật khẩu DB)
# Lưu ý: Thay đổi mật khẩu nếu muốn
echo "MSSQL_SA_PASSWORD=Password@12345" > .env

# 3. Build và Chạy Web App + Database
docker-compose up -d --build webapp
Sau khi chạy xong, truy cập website tại: http://localhost:8080

3. Dừng hệ thống
Bash

docker-compose down
🧪 Hướng dẫn chạy Test
Dự án bao gồm 2 bộ kiểm thử tự động:

1. Unit Tests (Chạy trong Docker)
Kiểm tra logic xử lý nội bộ (Services, Repositories).

Bash

# Lệnh này sẽ dựng môi trường test cô lập và chạy test
docker-compose --profile testing up --build unit-tests
2. E2E Tests (Selenium)
Kiểm tra luồng người dùng thật trên trình duyệt Edge. Yêu cầu: Website phải đang chạy tại localhost:8080.

Bash

# 1. Đảm bảo Web App đang chạy
docker-compose up -d webapp

# 2. Chạy test từ máy host (Yêu cầu cài .NET 9 SDK trên máy)
dotnet test ALittleLeaf.FunctionalTests
⚙️ CI/CD (GitHub Actions)
Dự án đã được thiết lập quy trình Continuous Integration (CI) tự động. Mỗi khi có code mới được đẩy lên nhánh master, GitHub Actions sẽ:

☁️ Checkout Code.

🐳 Dựng môi trường Docker (Web & SQL Server).

🗄️ Khởi tạo Database (Chạy script SQL mẫu).

✅ Chạy Unit Tests tự động.

Trạng thái Build & Test có thể xem trực tiếp tại tab Actions trên GitHub.

📂 Cấu trúc thư mục
ALittleLeaf/
├── ALittleLeaf/                 # Source code chính (Web App)
│   ├── Controllers/             # Presentation Layer
│   ├── Services/                # Business Logic Layer
│   ├── Repositories/            # Data Access Layer
│   ├── Views/                   # UI
│   └── Dockerfile               # Cấu hình Docker cho Web
├── ALittleLeaf.Tests/           # Unit Tests Project (xUnit)
├── ALittleLeaf.FunctionalTests/ # E2E Tests Project (Selenium)
├── .github/workflows/           # Cấu hình GitHub Actions
├── docker-compose.yml           # Định nghĩa các Services (Web, DB, Test)
├── Dockerfile.test              # Cấu hình Docker cho Unit Test
└── README.md                    # Tài liệu dự án
© 2025 ALittleLeaf Project. 
