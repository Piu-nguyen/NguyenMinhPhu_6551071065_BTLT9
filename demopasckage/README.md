# Demo tạo và xuất báo cáo — Chương 9

Tích hợp mã nguồn [Tao_Xuat_Report_Chap9](https://github.com/DuongNT1998/VisualProgramming_UTC2/tree/master/Tao_Xuat_Report_Chap9) vào ứng dụng WinForms `demopasckage` (.NET 10).

## Chạy ứng dụng

Mở `demopasckage.slnx` trong Visual Studio, chọn `demopasckage` làm startup project và nhấn **F5**, hoặc chạy:

```powershell
dotnet run --project demopasckage/demopasckage.csproj
```

Kết nối được cấu hình tại `demopasckage/AppConfig.cs`:

```text
Server=localhost;Database=ReportDemoDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True
```

Ứng dụng sử dụng tài khoản Windows đang chạy chương trình. Database đã có dữ liệu thì có thể chạy ứng dụng ngay.

## Các màn hình

- **Sản phẩm:** tìm kiếm tên, lọc danh mục; xuất Excel từ DataGridView, từ EF Core (công thức giá trị tồn và tổng), từ DataTable; xem và thiết kế báo cáo bảng giá.
- **Hóa đơn:** lọc theo ngày, xem chi tiết; xem báo cáo, xuất PDF, thiết kế mẫu, in bằng FastReport; xem trước, cài đặt trang và in nhiều trang bằng PrintDocument.
- **Doanh thu:** lọc theo ngày, xem chi tiết và tổng hợp danh mục bằng EF Core; báo cáo nhóm danh mục với Sum/Count/Avg, xuất PDF và Excel, thiết kế mẫu.

Hóa đơn và doanh thu mặc định lấy 90 ngày gần nhất. Khi cần xem dữ liệu cũ, đổi **Từ ngày / Đến ngày** rồi bấm **Tải danh sách / Tải dữ liệu**.

## Mẫu báo cáo và thư viện

Ba mẫu nằm tại `demopasckage/Reports`: `BangGia.frx`, `HoaDon.frx`, `DoanhThu.frx`. Khi chạy từ source, Designer lưu mẫu vào thư mục này. Các mẫu cũng được sao chép vào thư mục output/publish.

Dùng `FastReport.Net.Demo` như dự án nguồn để có Designer, Preview, PDF và chức năng in. Đây là bản demo của FastReport nên báo cáo có thể có dấu/bản quyền demo. ClosedXML dùng để xuất `.xlsx`, EF Core SQL Server dùng để đọc dữ liệu.

`demopasckage/Database/Tao_Xuat_Report_Chap9.sql` là script dữ liệu mẫu lấy từ dự án nguồn. Script tạo lại các bảng và dữ liệu; chỉ dùng khi cần khởi tạo lại database mẫu.
