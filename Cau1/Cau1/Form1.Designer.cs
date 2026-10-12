#nullable enable

namespace Cau1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer? components;
        private TextBox txtTenHang = null!;
        private ComboBox cboLoaiHang = null!;
        private ComboBox cboDonVi = null!;
        private NumericUpDown nudGiaBan = null!;
        private NumericUpDown nudSoLuongTon = null!;
        private DateTimePicker dtpNgayNhap = null!;
        private CheckBox chkConKinhDoanh = null!;
        private TextBox txtTimKiem = null!;
        private ComboBox cboLocLoai = null!;
        private CheckBox chkChiConKinhDoanh = null!;
        private DataGridView dgvHangHoa = null!;
        private FlowLayoutPanel pnlButtons = null!;
        private ToolStripStatusLabel lblTrangThai = null!;
        private System.Windows.Forms.Timer searchTimer = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            searchTimer = new System.Windows.Forms.Timer(components) { Interval = 350 };
            SuspendLayout();
            Font = new Font("Segoe UI", 10F);
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1200, 720);
            MinimumSize = new Size(1120, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý hàng hóa – Tạp hóa Nhà trường UTC2";

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top, Size = new Size(1200, 245), Padding = new Padding(12),
                ColumnCount = 2, RowCount = 1
            };
            top.SuspendLayout();
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            var info = new GroupBox { Text = "Thông tin hàng hóa", Size = new Size(758, 215), Dock = DockStyle.Fill };
            var search = new GroupBox { Text = "Tìm kiếm", Size = new Size(406, 215), Dock = DockStyle.Fill };
            top.Controls.Add(info, 0, 0);
            top.Controls.Add(search, 1, 0);

            txtTenHang = new TextBox { Location = new Point(112, 32), Width = 610, MaxLength = 100, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            cboLoaiHang = new ComboBox { Location = new Point(112, 77), Width = 235, DropDownStyle = ComboBoxStyle.DropDownList };
            cboDonVi = new ComboBox { Location = new Point(112, 121), Width = 235, DropDownStyle = ComboBoxStyle.DropDownList };
            nudGiaBan = new NumericUpDown { Location = new Point(112, 165), Width = 235, Maximum = 999999999999999999M, Increment = 500, ThousandsSeparator = true };
            nudSoLuongTon = new NumericUpDown { Location = new Point(504, 77), Width = 218, Maximum = int.MaxValue, ThousandsSeparator = true, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            dtpNgayNhap = new DateTimePicker { Location = new Point(504, 121), Width = 218, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", MinDate = new DateTime(1753, 1, 1), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            chkConKinhDoanh = new CheckBox { Text = "Còn kinh doanh", Location = new Point(504, 166), AutoSize = true, Checked = true, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            info.Controls.AddRange(new Control[]
            {
                InputLabel("Tên hàng:", 16, 35), txtTenHang,
                InputLabel("Loại hàng:", 16, 80), cboLoaiHang,
                InputLabel("Đơn vị:", 16, 124), cboDonVi,
                InputLabel("Giá bán:", 16, 168), nudGiaBan,
                InputLabel("Số lượng tồn:", 375, 80, true), nudSoLuongTon,
                InputLabel("Ngày nhập:", 375, 124, true), dtpNgayNhap, chkConKinhDoanh
            });

            txtTimKiem = new TextBox { Location = new Point(18, 58), Width = 355, PlaceholderText = "Nhập tên hàng cần tìm…", Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            cboLocLoai = new ComboBox { Location = new Point(18, 102), Width = 355, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            chkChiConKinhDoanh = new CheckBox { Text = "Chỉ hiện hàng còn kinh doanh", Location = new Point(18, 151), AutoSize = true };
            search.Controls.AddRange(new Control[] { InputLabel("Tìm theo tên hàng:", 18, 30), txtTimKiem, cboLocLoai, chkChiConKinhDoanh });

            dgvHangHoa = new DataGridView
            {
                Dock = DockStyle.Fill, AutoGenerateColumns = false,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, RowHeadersWidth = 35,
                AllowUserToOrderColumns = true
            };
            dgvHangHoa.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
            dgvHangHoa.RowTemplate.Height = 32;
            dgvHangHoa.Columns.AddRange(
                TextColumn("MaHang", "Mã", 45), TextColumn("TenHang", "Tên hàng", 240),
                TextColumn("LoaiHang", "Loại hàng", 140), TextColumn("DonVi", "Đơn vị", 65),
                TextColumn("GiaBan", "Giá bán", 105, "N0", true),
                TextColumn("SoLuongTon", "Tồn", 75, "N0", true),
                TextColumn("NgayNhap", "Ngày nhập", 110, "dd/MM/yyyy"),
                new DataGridViewCheckBoxColumn { Name = "ConKinhDoanh", DataPropertyName = "ConKinhDoanh", HeaderText = "Kinh doanh", FillWeight = 95 });
            var gridPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 0, 12, 8) };
            gridPanel.Controls.Add(dgvHangHoa);

            pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 62, Padding = new Padding(10, 8, 10, 8), WrapContents = false };
            AddButton("Thêm", 82, async (_, _) => await RunAsync(AddAsync));
            AddButton("Sửa", 82, async (_, _) => await RunAsync(UpdateAsync));
            AddButton("Xóa", 82, async (_, _) => await RunAsync(DeleteAsync));
            AddButton("Làm mới", 100, async (_, _) => await RunAsync(ResetAsync));
            AddButton("Xem bảng giá", 138, async (_, _) => await RunAsync(PreviewAsync));
            AddButton("Xuất PDF", 105, async (_, _) => await RunAsync(ExportPdfAsync));
            AddButton("Xuất Excel (lưới)", 158, async (_, _) => await RunAsync(ExportGridAsync));
            AddButton("Xuất Excel tồn kho", 177, async (_, _) => await RunAsync(ExportInventoryAsync));
            var status = new StatusStrip();
            lblTrangThai = new ToolStripStatusLabel("Đang tải dữ liệu…");
            status.Items.Add(lblTrangThai);

            Controls.Add(gridPanel);
            Controls.Add(top);
            Controls.Add(pnlButtons);
            Controls.Add(status);
            top.ResumeLayout(true);
            ResumeLayout(true);
        }

        private static Label InputLabel(string text, int x, int y, bool right = false) => new()
        {
            Text = text, AutoSize = true, Location = new Point(x, y),
            Anchor = AnchorStyles.Top | (right ? AnchorStyles.Right : AnchorStyles.Left)
        };

        private static DataGridViewTextBoxColumn TextColumn(string name, string title, int weight, string format = "", bool numeric = false) => new()
        {
            Name = name, DataPropertyName = name, HeaderText = title, FillWeight = weight,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = format,
                Alignment = numeric ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft
            }
        };

        private void AddButton(string text, int width, EventHandler handler)
        {
            var button = new Button { Text = text, Width = width, Height = 36, UseVisualStyleBackColor = true };
            button.Click += handler;
            pnlButtons.Controls.Add(button);
        }
    }
}
