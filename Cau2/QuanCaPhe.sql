/*
    BÀI 2 - QUÁN CÀ PHÊ GÓC PHỐ
    Mở file bằng SQL Server Management Studio (SSMS), chạy toàn bộ bằng F5.
    Dùng được với SQL Server hoặc SQL Server LocalDB.
    Script tạo CSDL, 3 bảng và dữ liệu mẫu đúng theo đề bài.
    Không xóa hay ghi đè dữ liệu nếu các bảng đã tồn tại.
*/

USE master;
GO

IF DB_ID(N'QuanCaPhe') IS NULL
BEGIN
    CREATE DATABASE QuanCaPhe;
END;
GO

USE QuanCaPhe;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.Mon', N'U') IS NOT NULL
       OR OBJECT_ID(N'dbo.HoaDon', N'U') IS NOT NULL
       OR OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NOT NULL
    BEGIN
        THROW 50001, N'Các bảng của bài 2 đã tồn tại. Script dừng để giữ nguyên dữ liệu hiện có.', 1;
    END;

    CREATE TABLE dbo.Mon
    (
        MaMon INT NOT NULL,
        TenMon NVARCHAR(100) NOT NULL,
        NhomMon NVARCHAR(30) NOT NULL,
        DonGia DECIMAL(18, 0) NOT NULL,
        CONSTRAINT PK_Mon PRIMARY KEY (MaMon),
        CONSTRAINT CK_Mon_DonGia CHECK (DonGia > 0)
    );

    CREATE TABLE dbo.HoaDon
    (
        MaHD INT IDENTITY(1, 1) NOT NULL,
        NgayLap DATETIME NOT NULL,
        SoBan INT NOT NULL,
        HinhThucTT NVARCHAR(20) NOT NULL,
        GhiChu NVARCHAR(200) NULL,
        CONSTRAINT PK_HoaDon PRIMARY KEY (MaHD),
        CONSTRAINT CK_HoaDon_SoBan CHECK (SoBan BETWEEN 1 AND 10),
        CONSTRAINT CK_HoaDon_HinhThucTT
            CHECK (HinhThucTT IN (N'Tiền mặt', N'Chuyển khoản'))
    );

    CREATE TABLE dbo.ChiTietHoaDon
    (
        MaCT INT IDENTITY(1, 1) NOT NULL,
        MaHD INT NOT NULL,
        MaMon INT NOT NULL,
        SoLuong INT NOT NULL,
        DonGia DECIMAL(18, 0) NOT NULL,
        CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaCT),
        CONSTRAINT FK_ChiTietHoaDon_HoaDon
            FOREIGN KEY (MaHD) REFERENCES dbo.HoaDon(MaHD),
        CONSTRAINT FK_ChiTietHoaDon_Mon
            FOREIGN KEY (MaMon) REFERENCES dbo.Mon(MaMon),
        CONSTRAINT CK_ChiTietHoaDon_SoLuong CHECK (SoLuong > 0),
        CONSTRAINT CK_ChiTietHoaDon_DonGia CHECK (DonGia > 0)
    );

    -- Không dùng ON DELETE CASCADE: ứng dụng xóa chi tiết trước rồi xóa hóa đơn.
    CREATE INDEX IX_ChiTietHoaDon_MaHD ON dbo.ChiTietHoaDon(MaHD);
    CREATE INDEX IX_ChiTietHoaDon_MaMon ON dbo.ChiTietHoaDon(MaMon);
    CREATE INDEX IX_HoaDon_NgayLap ON dbo.HoaDon(NgayLap);

    -- MaMon là khóa chính thường, không tự tăng theo đề bài.
    INSERT INTO dbo.Mon (MaMon, TenMon, NhomMon, DonGia)
    VALUES
        (1, N'Cà phê sữa đá', N'Cà phê', 29000),
        (2, N'Bạc xỉu', N'Cà phê', 32000),
        (3, N'Trà đào cam sả', N'Trà', 45000),
        (4, N'Bánh tiramisu', N'Bánh', 39000),
        (5, N'Nước suối', N'Nước đóng chai', 12000);

    -- Chuỗi ngày dạng ISO 8601, không phụ thuộc định dạng ngày của SQL Server.
    INSERT INTO dbo.HoaDon (NgayLap, SoBan, HinhThucTT, GhiChu)
    VALUES
        ('2026-10-09T08:15:00', 3, N'Tiền mặt', NULL),
        ('2026-10-09T09:40:00', 5, N'Chuyển khoản', NULL);

    -- Sao chép giá niêm yết sang giá bán của từng dòng chi tiết.
    INSERT INTO dbo.ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGia)
    SELECT mau.MaHD, mau.MaMon, mau.SoLuong, mon.DonGia
    FROM (VALUES
        (1, 1, 2),
        (1, 3, 1),
        (1, 4, 1),
        (2, 2, 1),
        (2, 5, 2)
    ) AS mau(MaHD, MaMon, SoLuong)
    INNER JOIN dbo.Mon AS mon ON mon.MaMon = mau.MaMon;

    COMMIT TRANSACTION;
    PRINT N'Đã tạo thành công CSDL QuanCaPhe và dữ liệu mẫu Câu 2.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- Kiểm tra: 5 món, 2 hóa đơn, 5 dòng chi tiết.
SELECT * FROM dbo.Mon ORDER BY MaMon;

-- Tổng tiền tính bằng truy vấn, không lưu trong bảng HoaDon.
-- Kết quả mong đợi: HD000001 = 142000; HD000002 = 56000.
SELECT
    N'HD' + FORMAT(hd.MaHD, '000000') AS SoHoaDon,
    hd.NgayLap,
    hd.SoBan,
    hd.HinhThucTT,
    hd.GhiChu,
    COALESCE(SUM(ct.SoLuong * ct.DonGia), 0) AS TongTien
FROM dbo.HoaDon AS hd
LEFT JOIN dbo.ChiTietHoaDon AS ct ON ct.MaHD = hd.MaHD
GROUP BY hd.MaHD, hd.NgayLap, hd.SoBan, hd.HinhThucTT, hd.GhiChu
ORDER BY hd.MaHD;

SELECT
    ct.MaCT,
    N'HD' + FORMAT(ct.MaHD, '000000') AS SoHoaDon,
    mon.TenMon,
    ct.SoLuong,
    ct.DonGia,
    ct.SoLuong * ct.DonGia AS ThanhTien
FROM dbo.ChiTietHoaDon AS ct
INNER JOIN dbo.Mon AS mon ON mon.MaMon = ct.MaMon
ORDER BY ct.MaHD, ct.MaCT;
GO
