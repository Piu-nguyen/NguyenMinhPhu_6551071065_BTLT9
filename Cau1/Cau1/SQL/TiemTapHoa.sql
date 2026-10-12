-- Câu 1: Tiệm tạp hóa của Nhà trường UTC2.
-- Mở file bằng SQL Server Management Studio (SSMS), chạy toàn bộ bằng F5.
-- Dùng được với SQL Server và SQL Server LocalDB.
-- Chạy lại không xóa dữ liệu và không chèn trùng dữ liệu mẫu.

USE master;
GO

IF DB_ID(N'TiemTapHoa') IS NULL
BEGIN
    CREATE DATABASE TiemTapHoa;
END;
GO

USE TiemTapHoa;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.HangHoa', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.HangHoa
        (
            MaHang INT IDENTITY(1, 1) NOT NULL,
            TenHang NVARCHAR(100) NOT NULL,
            LoaiHang NVARCHAR(50) NOT NULL,
            DonVi NVARCHAR(20) NOT NULL,
            GiaBan DECIMAL(18, 0) NOT NULL,
            SoLuongTon INT NOT NULL
                CONSTRAINT DF_HangHoa_SoLuongTon DEFAULT (0),
            NgayNhap DATETIME NOT NULL
                CONSTRAINT DF_HangHoa_NgayNhap DEFAULT (GETDATE()),
            ConKinhDoanh BIT NOT NULL
                CONSTRAINT DF_HangHoa_ConKinhDoanh DEFAULT (1),

            CONSTRAINT PK_HangHoa PRIMARY KEY (MaHang),
            CONSTRAINT CK_HangHoa_TenHang
                CHECK (LEN(LTRIM(RTRIM(TenHang))) > 0),
            CONSTRAINT CK_HangHoa_GiaBan CHECK (GiaBan > 0),
            CONSTRAINT CK_HangHoa_SoLuongTon CHECK (SoLuongTon >= 0)
        );

        -- Ngày theo dạng YYYYMMDD để không phụ thuộc ngôn ngữ SQL Server.
        -- Chuỗi tiếng Việt dùng tiền tố N để giữ nguyên dấu.
        INSERT INTO dbo.HangHoa
            (TenHang, LoaiHang, DonVi, GiaBan, SoLuongTon, NgayNhap, ConKinhDoanh)
        VALUES
            (N'Nước suối Aquafina 500ml', N'Đồ uống', N'Chai',
                6000, 120, '20261002', 1),
            (N'Mì Hảo Hảo tôm chua cay', N'Thực phẩm khô', N'Gói',
                4500, 200, '20261002', 1),
            (N'Dầu ăn Tường An 1L', N'Gia vị', N'Chai',
                48000, 35, '20261005', 1),
            (N'Bột giặt OMO 800g', N'Đồ gia dụng', N'Túi',
                52000, 40, '20261005', 1),
            (N'Sữa tươi Vinamilk 1L', N'Đồ uống', N'Hộp',
                32000, 60, '20261008', 1),
            (N'Nước mắm Nam Ngư 500ml', N'Gia vị', N'Chai',
                38000, 25, '20261001', 0);

        PRINT N'Đã tạo bảng HangHoa và thêm 6 mặt hàng mẫu.';
    END
    ELSE
    BEGIN
        PRINT N'Bảng HangHoa đã tồn tại. Giữ nguyên cấu trúc và dữ liệu hiện có.';
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- Kiểm tra kết quả: lần tạo đầu tiên có 6 mặt hàng.
SELECT MaHang, TenHang, LoaiHang, DonVi, GiaBan,
       SoLuongTon, NgayNhap, ConKinhDoanh
FROM dbo.HangHoa
ORDER BY MaHang;

-- Dữ liệu mẫu: 5 mặt hàng còn kinh doanh, tổng giá trị tồn = 7300000.
SELECT COUNT(*) AS SoMatHangConKinhDoanh,
       SUM(GiaBan * SoLuongTon) AS TongGiaTriTon
FROM dbo.HangHoa
WHERE ConKinhDoanh = 1;
GO
