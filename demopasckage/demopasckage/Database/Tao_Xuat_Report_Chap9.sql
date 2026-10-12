/* =====================================================================
   ReportDemoDb - CSDL mẫu cho demo Chương 9 (Report & Xuất tài liệu)
   Chạy được nhiều lần: mỗi lần chạy sẽ xóa bảng cũ và tạo lại dữ liệu.
   Cách chạy:
     - SSMS / Azure Data Studio: kết nối localhost rồi Execute
     - Dòng lệnh: sqlcmd -S localhost -E -C -f 65001 -i Tao_Xuat_Report_Chap9.sql
   ===================================================================== */
IF DB_ID(N'ReportDemoDb') IS NULL
    CREATE DATABASE ReportDemoDb;
GO

USE ReportDemoDb;
GO

IF OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NOT NULL DROP TABLE dbo.ChiTietHoaDon;
IF OBJECT_ID(N'dbo.HoaDon',        N'U') IS NOT NULL DROP TABLE dbo.HoaDon;
IF OBJECT_ID(N'dbo.SanPham',       N'U') IS NOT NULL DROP TABLE dbo.SanPham;
IF OBJECT_ID(N'dbo.KhachHang',     N'U') IS NOT NULL DROP TABLE dbo.KhachHang;
IF OBJECT_ID(N'dbo.DanhMuc',       N'U') IS NOT NULL DROP TABLE dbo.DanhMuc;
GO

CREATE TABLE dbo.DanhMuc (
    MaDanhMuc  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DanhMuc PRIMARY KEY,
    TenDanhMuc NVARCHAR(100)     NOT NULL
);

CREATE TABLE dbo.SanPham (
    MaSanPham  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SanPham PRIMARY KEY,
    TenSanPham NVARCHAR(150)     NOT NULL,
    MaDanhMuc  INT               NOT NULL CONSTRAINT FK_SanPham_DanhMuc REFERENCES dbo.DanhMuc(MaDanhMuc),
    DonGia     DECIMAL(18,0)     NOT NULL,
    SoLuongTon INT               NOT NULL CONSTRAINT DF_SanPham_Ton DEFAULT 0
);

CREATE TABLE dbo.KhachHang (
    MaKhachHang  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhachHang PRIMARY KEY,
    TenKhachHang NVARCHAR(150)     NOT NULL,
    DienThoai    VARCHAR(20)       NULL,
    DiaChi       NVARCHAR(200)     NULL
);

CREATE TABLE dbo.HoaDon (
    MaHoaDon    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HoaDon PRIMARY KEY,
    NgayLap     DATETIME2(0)      NOT NULL,
    MaKhachHang INT               NOT NULL CONSTRAINT FK_HoaDon_KhachHang REFERENCES dbo.KhachHang(MaKhachHang),
    GhiChu      NVARCHAR(200)     NULL
);

CREATE TABLE dbo.ChiTietHoaDon (
    MaChiTiet INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY,
    MaHoaDon  INT               NOT NULL CONSTRAINT FK_ChiTietHoaDon_HoaDon  REFERENCES dbo.HoaDon(MaHoaDon),
    MaSanPham INT               NOT NULL CONSTRAINT FK_ChiTietHoaDon_SanPham REFERENCES dbo.SanPham(MaSanPham),
    SoLuong   INT               NOT NULL,
    DonGia    DECIMAL(18,0)     NOT NULL   -- đơn giá TẠI THỜI ĐIỂM BÁN (không phụ thuộc giá hiện tại)
);
GO

/* ---------------------------- Dữ liệu mẫu ---------------------------- */
INSERT INTO dbo.DanhMuc (TenDanhMuc) VALUES
 (N'Đồ uống'), (N'Bánh kẹo'), (N'Gia dụng'), (N'Văn phòng phẩm');

-- Thứ tự INSERT quyết định mã (1..24): 1-6 Đồ uống, 7-12 Bánh kẹo, 13-18 Gia dụng, 19-24 VPP
INSERT INTO dbo.SanPham (TenSanPham, MaDanhMuc, DonGia, SoLuongTon) VALUES
 (N'Nước suối Aquafina 500ml',   1,   6000, 240),
 (N'Coca-Cola lon 330ml',        1,  10000, 180),
 (N'Trà xanh không độ 450ml',    1,  12000, 150),
 (N'Cà phê sữa lon 180ml',       1,  15000, 120),
 (N'Nước cam ép 1 lít',          1,  35000,  60),
 (N'Sữa tươi tiệt trùng 1 lít',  1,  32000,  90),
 (N'Bánh quy bơ 150g',           2,  28000,  75),
 (N'Kẹo dẻo trái cây 100g',      2,  18000, 110),
 (N'Snack khoai tây 52g',        2,  12000, 200),
 (N'Socola thanh 40g',           2,  25000,  80),
 (N'Bánh bông lan 200g',         2,  30000,  45),
 (N'Kẹo cao su bạc hà',          2,   8000, 300),
 (N'Chảo chống dính 24cm',       3, 185000,  25),
 (N'Bình giữ nhiệt 500ml',       3, 220000,  18),
 (N'Hộp cơm thủy tinh 800ml',    3, 165000,  30),
 (N'Khăn tắm cotton',            3,  95000,  40),
 (N'Bộ ly thủy tinh 6 cái',      3, 120000,  22),
 (N'Ổ cắm điện 3 lỗ',            3,  85000,  35),
 (N'Bút bi Thiên Long',          4,   5000, 500),
 (N'Vở kẻ ngang 200 trang',      4,  18000, 260),
 (N'Sổ tay A5',                  4,  45000,  70),
 (N'Kẹp giấy hộp 100 cái',       4,  12000, 150),
 (N'Giấy A4 Double A (ream)',    4,  98000,  55),
 (N'Bút dạ quang bộ 4 màu',      4,  40000,  65);

INSERT INTO dbo.KhachHang (TenKhachHang, DienThoai, DiaChi) VALUES
 (N'Nguyễn Văn An',              '0901234567', N'12 Lê Lợi, Quận 1, TP.HCM'),
 (N'Trần Thị Bích',              '0912345678', N'45 Nguyễn Huệ, Quận 1, TP.HCM'),
 (N'Lê Hoàng Cường',             '0987654321', N'78 Trần Hưng Đạo, Quận 5, TP.HCM'),
 (N'Phạm Minh Đức',              '0933222111', N'9 Hai Bà Trưng, Quận 3, TP.HCM'),
 (N'Công ty TNHH Thương mại Ánh Dương', '02838123456', N'256 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM');

-- Ngày lập tính lùi từ hôm nay để dữ liệu luôn "mới" khi demo trên lớp
INSERT INTO dbo.HoaDon (NgayLap, MaKhachHang, GhiChu) VALUES
 (DATEADD(DAY, -85, DATEADD(HOUR, 9,  CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 1, NULL),
 (DATEADD(DAY, -72, DATEADD(HOUR, 10, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 2, NULL),
 (DATEADD(DAY, -60, DATEADD(HOUR, 14, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 3, NULL),
 (DATEADD(DAY, -48, DATEADD(HOUR, 11, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 1, NULL),
 (DATEADD(DAY, -41, DATEADD(HOUR, 15, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 4, NULL),
 (DATEADD(DAY, -33, DATEADD(HOUR, 8,  CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 5, N'Xuất hóa đơn công ty'),
 (DATEADD(DAY, -27, DATEADD(HOUR, 16, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 2, NULL),
 (DATEADD(DAY, -19, DATEADD(HOUR, 13, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 3, NULL),
 (DATEADD(DAY, -12, DATEADD(HOUR, 9,  CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 4, NULL),
 (DATEADD(DAY,  -6, DATEADD(HOUR, 17, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 5, N'Giao hàng trong ngày'),
 (DATEADD(DAY,  -2, DATEADD(HOUR, 10, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 1, N'Giao hàng trước 17h'),
 (DATEADD(DAY,  -1, DATEADD(HOUR, 11, CAST(CAST(GETDATE() AS date) AS datetime2(0)))), 5, N'Hóa đơn nhiều dòng - dùng để thử in nhiều trang');

-- Chi tiết: đơn giá lấy từ SanPham.DonGia tại thời điểm chèn
INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaSanPham, SoLuong, DonGia)
SELECT v.MaHoaDon, v.MaSanPham, v.SoLuong, sp.DonGia
FROM (VALUES
  ( 1, 1,10), ( 1, 2, 6), ( 1, 9, 4), ( 1, 8, 3),
  ( 2, 7, 2), ( 2,10, 5), ( 2,12,10),
  ( 3,13, 1), ( 3,14, 1), ( 3,16, 2),
  ( 4,19,20), ( 4,20,10), ( 4,23, 5), ( 4,24, 3),
  ( 5, 3, 6), ( 5, 4, 6), ( 5, 5, 2), ( 5, 6, 4),
  ( 6,15, 2), ( 6,17, 1), ( 6,18, 3),
  ( 7,21, 5), ( 7,22, 5), ( 7,19,10),
  ( 8, 2,12), ( 8, 9, 6), ( 8,11, 2), ( 8, 1,12),
  ( 9,14, 2), ( 9,13, 2), ( 9,16, 6),
  (10,20,30), (10,21,10), (10,22, 8), (10,24, 6),
  (11, 5, 3), (11, 6, 3), (11,10, 2), (11, 7, 1), (11,11, 1)
) AS v(MaHoaDon, MaSanPham, SoLuong)
JOIN dbo.SanPham sp ON sp.MaSanPham = v.MaSanPham;

-- Hóa đơn số 12: mỗi sản phẩm xuất hiện 2 dòng (24 x 2 = 48 dòng) -> in ra > 1 trang
INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaSanPham, SoLuong, DonGia)
SELECT 12, sp.MaSanPham, 1 + (sp.MaSanPham % 3), sp.DonGia FROM dbo.SanPham sp;
INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaSanPham, SoLuong, DonGia)
SELECT 12, sp.MaSanPham, 2, sp.DonGia FROM dbo.SanPham sp;
GO

-- Kiểm tra nhanh
SELECT (SELECT COUNT(*) FROM dbo.SanPham)       AS SoSanPham,
       (SELECT COUNT(*) FROM dbo.HoaDon)        AS SoHoaDon,
       (SELECT COUNT(*) FROM dbo.ChiTietHoaDon) AS SoDongChiTiet;
GO
