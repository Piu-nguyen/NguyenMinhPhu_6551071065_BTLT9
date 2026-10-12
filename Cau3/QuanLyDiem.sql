-- CAU 3: QUAN LY SINH VIEN VA PHIEU BAO DIEM
-- Mo file trong SQL Server Management Studio (SSMS), chon server va nhan F5.
-- Dung duoc voi SQL Server va SQL Server LocalDB.
-- Script khong xoa CSDL/bang da co; chi chen du lieu mau khi ca 3 bang rong.

USE [master];
GO

IF DB_ID(N'QuanLyDiem') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [QuanLyDiem] COLLATE Vietnamese_100_CI_AS;');
END;
GO

USE [QuanLyDiem];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.Lop', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Lop
        (
            MaLop varchar(10) NOT NULL,
            TenLop nvarchar(100) NOT NULL,
            CONSTRAINT PK_Lop PRIMARY KEY (MaLop),
            CONSTRAINT CK_Lop_MaLop CHECK (LEN(LTRIM(RTRIM(MaLop))) > 0),
            CONSTRAINT CK_Lop_TenLop CHECK (LEN(LTRIM(RTRIM(TenLop))) > 0)
        );
    END;

    IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.SinhVien
        (
            MaSV varchar(10) NOT NULL,
            HoTen nvarchar(100) COLLATE Vietnamese_100_CI_AS NOT NULL,
            NgaySinh datetime NOT NULL,
            GioiTinh nvarchar(3) NOT NULL,
            MaLop varchar(10) NOT NULL,
            CONSTRAINT PK_SinhVien PRIMARY KEY (MaSV),
            CONSTRAINT FK_SinhVien_Lop FOREIGN KEY (MaLop)
                REFERENCES dbo.Lop (MaLop),
            CONSTRAINT CK_SinhVien_MaSV CHECK (LEN(LTRIM(RTRIM(MaSV))) > 0),
            CONSTRAINT CK_SinhVien_HoTen CHECK (LEN(LTRIM(RTRIM(HoTen))) > 0),
            CONSTRAINT CK_SinhVien_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ'))
        );

        CREATE INDEX IX_SinhVien_MaLop ON dbo.SinhVien (MaLop);
    END;

    IF OBJECT_ID(N'dbo.DiemMon', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.DiemMon
        (
            MaDiem int IDENTITY(1,1) NOT NULL,
            MaSV varchar(10) NOT NULL,
            TenMon nvarchar(100) NOT NULL,
            SoTinChi int NOT NULL,
            Diem decimal(4,2) NOT NULL,
            CONSTRAINT PK_DiemMon PRIMARY KEY (MaDiem),
            CONSTRAINT FK_DiemMon_SinhVien FOREIGN KEY (MaSV)
                REFERENCES dbo.SinhVien (MaSV),
            CONSTRAINT CK_DiemMon_TenMon CHECK (LEN(LTRIM(RTRIM(TenMon))) > 0),
            CONSTRAINT CK_DiemMon_SoTinChi CHECK (SoTinChi BETWEEN 1 AND 6),
            CONSTRAINT CK_DiemMon_Diem CHECK (Diem BETWEEN 0 AND 10)
        );

        CREATE INDEX IX_DiemMon_MaSV ON dbo.DiemMon (MaSV);
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Lop)
       AND NOT EXISTS (SELECT 1 FROM dbo.SinhVien)
       AND NOT EXISTS (SELECT 1 FROM dbo.DiemMon)
    BEGIN
        INSERT INTO dbo.Lop (MaLop, TenLop)
        VALUES
            ('CNTT01', N'Công nghệ thông tin K01'),
            ('KT01', N'Kế toán K01');

        -- Ngay dang YYYYMMDD khong phu thuoc thiet lap ngon ngu cua SQL Server.
        INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, MaLop)
        VALUES
            ('SV001', N'Nguyễn Minh Anh', '20050315', N'Nữ', 'CNTT01'),
            ('SV002', N'Trần Quốc Bảo', '20050722', N'Nam', 'CNTT01'),
            ('SV003', N'Lê Thị Cẩm Tú', '20051109', N'Nữ', 'KT01');

        INSERT INTO dbo.DiemMon (MaSV, TenMon, SoTinChi, Diem)
        VALUES
            ('SV001', N'Lập trình trực quan', 3, 8.50),
            ('SV001', N'Cơ sở dữ liệu', 3, 7.50),
            ('SV001', N'Tiếng Anh chuyên ngành', 2, 9.00),
            ('SV001', N'Toán rời rạc', 3, 6.50),
            ('SV002', N'Lập trình trực quan', 3, 9.00),
            ('SV002', N'Cơ sở dữ liệu', 3, 8.00),
            ('SV002', N'Tiếng Anh chuyên ngành', 2, 8.50);

        -- SV003 chua co diem, dung theo de bai.
        PRINT N'Đã tạo bảng và chèn dữ liệu mẫu Câu 3.';
    END
    ELSE
    BEGIN
        PRINT N'CSDL đã có dữ liệu. Giữ nguyên dữ liệu hiện có.';
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- KIEM TRA: 2 lop, 3 sinh vien, 7 dong diem khi tao moi.
SELECT N'Lop' AS TenBang, COUNT(*) AS SoDong FROM dbo.Lop
UNION ALL
SELECT N'SinhVien', COUNT(*) FROM dbo.SinhVien
UNION ALL
SELECT N'DiemMon', COUNT(*) FROM dbo.DiemMon;

-- Diem TB co trong so: SV001 = 7.77 (Kha), SV002 = 8.50 (Gioi).
;WITH TongHop AS
(
    SELECT sv.MaSV, sv.HoTen, sv.MaLop,
           COALESCE(SUM(dm.SoTinChi), 0) AS TongTinChi,
           CAST(ROUND(SUM(dm.Diem * dm.SoTinChi)
                / NULLIF(SUM(dm.SoTinChi), 0), 2) AS decimal(4,2)) AS DiemTrungBinh
    FROM dbo.SinhVien AS sv
    LEFT JOIN dbo.DiemMon AS dm ON dm.MaSV = sv.MaSV
    GROUP BY sv.MaSV, sv.HoTen, sv.MaLop
)
SELECT MaSV, HoTen, MaLop, TongTinChi, DiemTrungBinh,
       CASE
           WHEN TongTinChi = 0 THEN N'Chưa có điểm'
           WHEN DiemTrungBinh >= 8 THEN N'Giỏi'
           WHEN DiemTrungBinh >= 7 THEN N'Khá'
           WHEN DiemTrungBinh >= 5 THEN N'Trung bình'
           ELSE N'Yếu'
       END AS XepLoai
FROM TongHop
ORDER BY MaSV;
GO
