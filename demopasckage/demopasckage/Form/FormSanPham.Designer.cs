using Tao_Xuat_Report_Chap9.Helpers;
using Tao_Xuat_Report_Chap9.Models;


namespace Tao_Xuat_Report_Chap9.Forms
{
    partial class FormSanPham
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.txtTimKiem = new TextBox { Width = 240, PlaceholderText = "Gõ để tìm theo tên sản phẩm…", Margin = new Padding(4, 6, 4, 4) };
            this.cboDanhMuc = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(4, 6, 4, 4) };
            var tren = UiHelper.DaiNut(UiHelper.Nhan("Tìm:"), txtTimKiem, UiHelper.Nhan("Danh mục:"), cboDanhMuc);

            this.grid = UiHelper.TaoLuoi();
            UiHelper.ThemCot(grid, nameof(SanPhamDong.MaSanPham), "Mã", 40, "0", true);
            UiHelper.ThemCot(grid, nameof(SanPhamDong.TenSanPham), "Tên sản phẩm", 220);
            UiHelper.ThemCot(grid, nameof(SanPhamDong.TenDanhMuc), "Danh mục", 110);
            UiHelper.ThemCot(grid, nameof(SanPhamDong.DonGia), "Đơn giá", 80, "N0", true);
            UiHelper.ThemCot(grid, nameof(SanPhamDong.SoLuongTon), "Tồn kho", 60, "N0", true);

            this.btnExcelLuoi = UiHelper.TaoNut("Excel ← từ DataGridView");
            this.btnExcelEf = UiHelper.TaoNut("Excel ← từ EF Core (có công thức)");
            this.btnExcelNhanh = UiHelper.TaoNut("Excel nhanh ← DataTable");
            this.btnBangGia = UiHelper.TaoNut("Xem report Bảng giá (DataTable)");
            this.btnThietKe = UiHelper.TaoNut("Thiết kế mẫu Bảng giá");
            var duoi = UiHelper.DaiNut(btnExcelLuoi, btnExcelEf, btnExcelNhanh, btnBangGia, btnThietKe);

            this.lblTrangThai = new Label { AutoSize = true, Padding = new Padding(8, 4, 8, 6) };

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 620);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sản phẩm — Xuất Excel & Report từ DataTable";

            this.Controls.Add(UiHelper.XepDoc((tren, false), (grid, true), (duoi, false), (lblTrangThai, false)));
        }

        #endregion

        private TextBox txtTimKiem;
        private ComboBox cboDanhMuc;
        private DataGridView grid;
        private Button btnExcelLuoi;
        private Button btnExcelEf;
        private Button btnExcelNhanh;
        private Button btnBangGia;
        private Button btnThietKe;
        private Label lblTrangThai;
    }
}