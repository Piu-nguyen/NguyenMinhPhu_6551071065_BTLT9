using System.Diagnostics;
using System.Drawing.Printing;
using Tao_Xuat_Report_Chap9.Services;

namespace Tao_Xuat_Report_Chap9.Helpers;

public static class UiHelper
{
    public static string? ChonFileLuu(string boLoc, string tenMacDinh)
    {
        using var dlg = new SaveFileDialog
        {
            Filter = boLoc,
            FileName = tenMacDinh,
            OverwritePrompt = true,
            Title = "Chọn nơi lưu file"
        };
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
    }

    public static void HoiMoFile(IWin32Window owner, string path)
    {
        var kq = MessageBox.Show(owner, $"Đã lưu file:\n{path}\n\nMở file ngay?", "Hoàn tất",
            MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        if (kq == DialogResult.Yes)
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    public static bool CoMauReport(IWin32Window owner, string tenFile)
    {
        if (ReportService.CoMau(tenFile)) return true;

        MessageBox.Show(owner,
            $"Chưa có mẫu report \"{tenFile}\".\n\n" +
            "Bấm nút [Thiết kế mẫu] để mở FastReport Designer, kéo thả field vào report rồi nhấn Save (Ctrl+S).",
            "Chưa có mẫu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    public static void Chay(IWin32Window owner, Action tacVu)
    {
        try
        {
            tacVu();
        }
        catch (Exception ex)
        {
            HienLoi(owner, ex);
        }
    }

    public static async void ChayAsync(IWin32Window owner, Func<Task> tacVu)
    {
        var ctl = owner as Control;
        try
        {
            if (ctl != null) ctl.Cursor = Cursors.WaitCursor;
            await tacVu();
        }
        catch (Exception ex)
        {
            HienLoi(owner, ex);
        }
        finally
        {
            if (ctl != null && !ctl.IsDisposed) ctl.Cursor = Cursors.Default;
        }
    }

    public static void HienLoi(IWin32Window owner, Exception ex)
    {
        var goc = ex.GetBaseException();
        string goiY = ex switch
        {
            InvalidPrinterException =>
                "\n\nGợi ý: máy tính chưa có máy in. Cài \"Microsoft Print to PDF\" (Windows Features) để thử in.",
            IOException =>
                "\n\nGợi ý: file có thể đang được mở bởi Excel/trình đọc PDF. Hãy đóng file rồi thử lại.",
            _ when goc is Microsoft.Data.SqlClient.SqlException =>
                "\n\nGợi ý: kiểm tra SQL Server localhost, database ReportDemoDb và quyền Windows Authentication; chuỗi kết nối nằm trong AppConfig.cs.",
            _ => ""
        };

        MessageBox.Show(owner, goc.Message + goiY, "Có lỗi xảy ra", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static Button TaoNut(string chu, int rong = 0)
    {
        var b = new Button
        {
            Text = chu,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(rong, 34),
            Padding = new Padding(8, 2, 8, 2),
            Margin = new Padding(4)
        };
        return b;
    }

    public static Label Nhan(string chu) => new()
    {
        Text = chu,
        AutoSize = true,
        Margin = new Padding(8, 9, 2, 0)
    };

    public static FlowLayoutPanel DaiNut(params Control[] cacNut)
    {
        var p = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            Padding = new Padding(4)
        };
        p.Controls.AddRange(cacNut);
        return p;
    }

    public static GroupBox Khung(string tieuDe, Control noiDung)
    {
        noiDung.Dock = DockStyle.Fill;
        var gb = new GroupBox { Text = tieuDe, Dock = DockStyle.Fill, Padding = new Padding(6) };
        gb.Controls.Add(noiDung);
        return gb;
    }

    public static DataGridView TaoLuoi() => new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        AutoGenerateColumns = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        RowHeadersVisible = false,
        BackgroundColor = SystemColors.Window,
        BorderStyle = BorderStyle.FixedSingle,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };

    public static void ThemCot(DataGridView grid, string tenProperty, string tieuDe, int doRong, string? dinhDang = null, bool canPhai = false)
    {
        var c = new DataGridViewTextBoxColumn
        {
            Name = "col" + tenProperty,
            DataPropertyName = tenProperty,
            HeaderText = tieuDe,
            FillWeight = doRong
        };
        if (dinhDang != null) c.DefaultCellStyle.Format = dinhDang;
        if (canPhai) c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        grid.Columns.Add(c);
    }

    public static TableLayoutPanel XepDoc(params (Control Ctl, bool Gian)[] cacHang)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = cacHang.Length };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < cacHang.Length; i++)
        {
            t.RowStyles.Add(cacHang[i].Gian ? new RowStyle(SizeType.Percent, 100) : new RowStyle(SizeType.AutoSize));
            cacHang[i].Ctl.Dock = DockStyle.Fill;
            t.Controls.Add(cacHang[i].Ctl, 0, i);
        }
        return t;
    }

    public static TableLayoutPanel XepNgang(params (Control Ctl, float PhanTram)[] cacCot)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = cacCot.Length, RowCount = 1 };
        t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        for (int i = 0; i < cacCot.Length; i++)
        {
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, cacCot[i].PhanTram));
            cacCot[i].Ctl.Dock = DockStyle.Fill;
            t.Controls.Add(cacCot[i].Ctl, i, 0);
        }
        return t;
    }

    public static DateTimePicker TaoChonNgay(DateTime giaTri) => new()
    {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "dd/MM/yyyy",
        Width = 120,
        Value = giaTri,
        Margin = new Padding(4, 6, 4, 4)
    };
}
