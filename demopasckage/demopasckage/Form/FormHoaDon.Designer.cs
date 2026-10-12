using Tao_Xuat_Report_Chap9.Helpers;
using Tao_Xuat_Report_Chap9.Models;


namespace Tao_Xuat_Report_Chap9.Forms
{
    partial class FormHoaDon
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

            this.dtpTu = UiHelper.TaoChonNgay(DateTime.Today.AddDays(-90));
            this.dtpDen = UiHelper.TaoChonNgay(DateTime.Today);
            this.btnTim = UiHelper.TaoNut("Tải danh sách");
            var tren = UiHelper.DaiNut(UiHelper.Nhan("Từ ngày:"), dtpTu, UiHelper.Nhan("Đến ngày:"), dtpDen, btnTim);

            this.gridHoaDon = UiHelper.TaoLuoi();
            UiHelper.ThemCot(gridHoaDon, nameof(HoaDonTomTat.MaHoaDon), "Mã HĐ", 45, "0", true);
            UiHelper.ThemCot(gridHoaDon, nameof(HoaDonTomTat.NgayLap), "Ngày lập", 85, "dd/MM/yyyy HH:mm");
            UiHelper.ThemCot(gridHoaDon, nameof(HoaDonTomTat.TenKhachHang), "Khách hàng", 140);
            UiHelper.ThemCot(gridHoaDon, nameof(HoaDonTomTat.SoDong), "Số dòng", 45, "N0", true);
            UiHelper.ThemCot(gridHoaDon, nameof(HoaDonTomTat.TongTien), "Tổng tiền", 80, "N0", true);

            this.gridChiTiet = UiHelper.TaoLuoi();
            UiHelper.ThemCot(gridChiTiet, nameof(ChiTietDong.STT), "STT", 30, "0", true);
            UiHelper.ThemCot(gridChiTiet, nameof(ChiTietDong.TenSanPham), "Tên sản phẩm", 160);
            UiHelper.ThemCot(gridChiTiet, nameof(ChiTietDong.SoLuong), "SL", 35, "N0", true);
            UiHelper.ThemCot(gridChiTiet, nameof(ChiTietDong.DonGia), "Đơn giá", 70, "N0", true);
            UiHelper.ThemCot(gridChiTiet, nameof(ChiTietDong.ThanhTien), "Thành tiền", 80, "N0", true);

            var giua = UiHelper.XepNgang(
                (UiHelper.Khung("Danh sách hóa đơn", gridHoaDon), 52f),
                (UiHelper.Khung("Chi tiết hóa đơn đang chọn", gridChiTiet), 48f));

            this.lblTong = new Label { AutoSize = true, Padding = new Padding(8, 4, 8, 4), Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            this.btnXem = UiHelper.TaoNut("Xem hóa đơn");
            this.btnPdf = UiHelper.TaoNut("Xuất PDF");
            this.btnInFr = UiHelper.TaoNut("In (FastReport)");
            this.btnThietKe = UiHelper.TaoNut("Thiết kế mẫu");
            this.btnPreview = UiHelper.TaoNut("Xem trước khi in");
            this.btnInDoc = UiHelper.TaoNut("In…");
            this.btnPageSetup = UiHelper.TaoNut("Cài đặt trang…");

            var duoi = UiHelper.DaiNut(
                UiHelper.Nhan("FastReport:"), btnXem, btnPdf, btnInFr, btnThietKe,
                UiHelper.Nhan("     PrintDocument:"), btnPreview, btnInDoc, btnPageSetup);

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hóa đơn — FastReport (List<T>) & PrintDocument";

            this.Controls.Add(UiHelper.XepDoc((tren, false), (giua, true), (lblTong, false), (duoi, false)));
        }

        #endregion

        private DateTimePicker dtpTu;
        private DateTimePicker dtpDen;
        private Button btnTim;
        private DataGridView gridHoaDon;
        private DataGridView gridChiTiet;
        private Label lblTong;

        private Button btnXem;
        private Button btnPdf;
        private Button btnInFr;
        private Button btnThietKe;

        private Button btnPreview;
        private Button btnInDoc;
        private Button btnPageSetup;
    }
}