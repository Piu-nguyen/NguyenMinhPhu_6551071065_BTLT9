
using Tao_Xuat_Report_Chap9.Forms;
using Tao_Xuat_Report_Chap9.Helpers;
using Tao_Xuat_Report_Chap9.Models;


/// <summary>Màn hình chính: 3 nút mở 3 form demo + dòng trạng thái kết nối CSDL.</summary>
public class FormMain : Form
{
    private readonly Label _lblDb = new() { AutoSize = true, Margin = new Padding(8) };

    public FormMain()
    {
        Text = "Demo Chương 9 — Report & Xuất tài liệu";
        ClientSize = new Size(520, 360);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var tieuDe = new Label
        {
            Text = "Chương 9 — Report & Xuất tài liệu",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(8, 12, 8, 8)
        };

        var btnSanPham = TaoNutLon("1. Sản phẩm\nExport Excel + Report từ DataTable");
        var btnHoaDon = TaoNutLon("2. Hóa đơn\nFastReport (List<T>) + PDF + In (PrintDocument)");
        var btnDoanhThu = TaoNutLon("3. Báo cáo doanh thu\nFastReport Group + Sum/Count/Avg, EF Core");

        btnSanPham.Click += (_, _) => Mo(new FormSanPham());
        btnHoaDon.Click += (_, _) => Mo(new FormHoaDon());
        btnDoanhThu.Click += (_, _) => Mo(new FormDoanhThu());

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(12)
        };
        flow.Controls.AddRange(new Control[] { tieuDe, btnSanPham, btnHoaDon, btnDoanhThu, _lblDb });
        Controls.Add(flow);

        Shown += (_, _) => UiHelper.ChayAsync(this, KiemTraKetNoiAsync);
    }

    private static Button TaoNutLon(string chu) => new()
    {
        Text = chu,
        Size = new Size(470, 62),
        Margin = new Padding(8, 4, 8, 4),
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(12, 0, 0, 0)
    };

    private void Mo(Form form)
    {
        using (form)
            form.ShowDialog(this);
    }

    private async Task KiemTraKetNoiAsync()
    {
        _lblDb.Text = "Đang kiểm tra kết nối CSDL…";
        await using var db = new ReportDemoDbContext();
        bool ok = await db.Database.CanConnectAsync();

        _lblDb.ForeColor = ok ? Color.DarkGreen : Color.Firebrick;
        _lblDb.Text = ok
            ? "Đã kết nối CSDL ReportDemoDb"
            : "Không kết nối được localhost / ReportDemoDb. Kiểm tra AppConfig.cs.";
    }
}
