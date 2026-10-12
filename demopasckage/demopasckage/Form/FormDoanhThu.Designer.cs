using Tao_Xuat_Report_Chap9.Helpers;
using Tao_Xuat_Report_Chap9.Models;

namespace Tao_Xuat_Report_Chap9.Forms
{
    partial class FormDoanhThu
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dtpTu = UiHelper.TaoChonNgay(DateTime.Today.AddDays(-90));
            this.dtpDen = UiHelper.TaoChonNgay(DateTime.Today);
            this.btnTai = UiHelper.TaoNut("Tải dữ liệu (EF Core)");

            var tren = UiHelper.DaiNut(
                UiHelper.Nhan("Từ ngày:"), dtpTu,
                UiHelper.Nhan("Đến ngày:"), dtpDen,
                btnTai
            );

            this.gridChiTiet = UiHelper.TaoLuoi();
            UiHelper.ThemCot(gridChiTiet, nameof(DoanhThuDong.TenDanhMuc), "Danh mục", 80);
            UiHelper.ThemCot(gridChiTiet, nameof(DoanhThuDong.TenSanPham), "Sản phẩm", 140);
            UiHelper.ThemCot(gridChiTiet, nameof(DoanhThuDong.MaHoaDon), "Mã HĐ", 45, "0", true);
            UiHelper.ThemCot(gridChiTiet, nameof(DoanhThuDong.NgayLap), "Ngày lập", 85, "dd/MM/yyyy");
            UiHelper.ThemCot(gridChiTiet, nameof(DoanhThuDong.SoLuong), "SL", 35, "N0", true);
            UiHelper.ThemCot(gridChiTiet, nameof(DoanhThuDong.ThanhTien), "Thành tiền", 80, "N0", true);

            this.gridTongHop = UiHelper.TaoLuoi();
            UiHelper.ThemCot(gridTongHop, nameof(TongHopDanhMuc.TenDanhMuc), "Danh mục", 90);
            UiHelper.ThemCot(gridTongHop, nameof(TongHopDanhMuc.SoDong), "Số dòng", 45, "N0", true);
            UiHelper.ThemCot(gridTongHop, nameof(TongHopDanhMuc.TongSoLuong), "Tổng SL", 50, "N0", true);
            UiHelper.ThemCot(gridTongHop, nameof(TongHopDanhMuc.TongTien), "Tổng tiền", 80, "N0", true);

            var giua = UiHelper.XepNgang(
                (UiHelper.Khung("Dữ liệu chi tiết (sẽ nạp vào report)", gridChiTiet), 62f),
                (UiHelper.Khung("Đối chiếu: GROUP BY bằng EF Core", gridTongHop), 38f)
            );

            this.lblTong = new Label
            {
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            this.btnXem = UiHelper.TaoNut("Xem báo cáo");
            this.btnPdf = UiHelper.TaoNut("Xuất PDF (từ report)");
            this.btnExcel = UiHelper.TaoNut("Xuất Excel (từ dữ liệu EF Core)");
            this.btnThietKe = UiHelper.TaoNut("Thiết kế mẫu");

            var duoi = UiHelper.DaiNut(btnXem, btnPdf, btnExcel, btnThietKe);

            // Cấu hình thuộc tính cho Form
            this.Text = "Báo cáo doanh thu — FastReport Group + Sum/Count/Avg";
            this.ClientSize = new Size(1180, 680);
            this.StartPosition = FormStartPosition.CenterParent;

            // Thêm bố cục chính vào Form
            this.Controls.Add(UiHelper.XepDoc(
                (tren, false),
                (giua, true),
                (lblTong, false),
                (duoi, false)
            ));
        }

        #endregion

        // Khai báo biến các Control chuẩn theo Designer
        private DateTimePicker dtpTu;
        private DateTimePicker dtpDen;
        private Button btnTai;
        private DataGridView gridChiTiet;
        private DataGridView gridTongHop;
        private Label lblTong;
        private Button btnXem;
        private Button btnPdf;
        private Button btnExcel;
        private Button btnThietKe;
    }
}