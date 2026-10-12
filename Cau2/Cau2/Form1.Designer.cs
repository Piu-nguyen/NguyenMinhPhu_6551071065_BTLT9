#nullable enable

namespace Cau2;

partial class Form1
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel contentPanel = null!;
    private DataGridView gridHoaDon = null!;
    private DataGridView gridChiTiet = null!;
    private GroupBox groupChiTiet = null!;
    private DateTimePicker dtpTuNgay = null!;
    private DateTimePicker dtpDenNgay = null!;
    private DateTimePicker dtpNgayLap = null!;
    private ComboBox cboLocBan = null!;
    private ComboBox cboLocTT = null!;
    private ComboBox cboSoBan = null!;
    private ComboBox cboMon = null!;
    private TextBox txtTenMon = null!;
    private TextBox txtGhiChu = null!;
    private RadioButton rdoTienMat = null!;
    private RadioButton rdoChuyenKhoan = null!;
    private NumericUpDown nudSoLuong = null!;
    private Label lblTongCong = null!;
    private Label lblDoanhThu = null!;
    private Label lblCount = null!;
    private Label lblStatus = null!;
    private Button btnTim = null!;
    private Button btnLamMoi = null!;
    private Button btnHoaDonMoi = null!;
    private Button btnSuaHoaDon = null!;
    private Button btnXoaHoaDon = null!;
    private Button btnThemMon = null!;
    private Button btnSuaSL = null!;
    private Button btnXoaMon = null!;
    private Button btnXem = null!;
    private Button btnPdf = null!;
    private Button btnIn = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        Name = "Form1";
        Text = "Quán cà phê Góc Phố - Quản lý hóa đơn";
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(1060, 565);
        MinimumSize = new Size(1076, 604);
        BackColor = SystemColors.Control;

        contentPanel = new TableLayoutPanel
        {
            Name = "contentPanel", Dock = DockStyle.Fill, Padding = new Padding(8),
            ColumnCount = 1, RowCount = 3, Margin = new Padding(0)
        };
        contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        var filterGroup = new GroupBox
        {
            Text = "Tìm kiếm hóa đơn", Dock = DockStyle.Fill, Padding = new Padding(10, 6, 10, 8)
        };
        var filters = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2 };
        foreach (float percent in new[] { 19F, 16F, 14F, 16F, 25F, 10F })
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
        filters.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        filters.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        dtpTuNgay = MakeDate("dtpTuNgay", "dd/MM/yyyy");
        dtpDenNgay = MakeDate("dtpDenNgay", "dd/MM/yyyy");
        dtpTuNgay.Width = 145;
        dtpDenNgay.Width = 145;
        cboLocBan = MakeCombo("cboLocBan");
        cboLocBan.Items.Add("Tất cả");
        for (int i = 1; i <= 10; i++) cboLocBan.Items.Add(i);
        cboLocBan.SelectedIndex = 0;
        cboLocTT = MakeCombo("cboLocTT");
        cboLocTT.Items.AddRange(["Tất cả", "Tiền mặt", "Chuyển khoản"]);
        cboLocTT.SelectedIndex = 0;
        txtTenMon = new TextBox { Name = "txtTenMon", Dock = DockStyle.Top };
        Control[] filterControls = [dtpTuNgay, dtpDenNgay, cboLocBan, cboLocTT, txtTenMon];
        string[] captions = ["Từ ngày", "Đến ngày", "Số bàn", "Hình thức", "Tên món"];
        for (int i = 0; i < filterControls.Length; i++)
        {
            filters.Controls.Add(MakeLabel(captions[i]), i, 0);
            filters.Controls.Add(filterControls[i], i, 1);
        }
        btnTim = MakeButton("btnTim", "Tìm");
        filters.Controls.Add(btnTim, 5, 1);
        filterGroup.Controls.Add(filters);
        contentPanel.Controls.Add(filterGroup, 0, 0);

        var masterDetail = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0, 4, 0, 0)
        };
        masterDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        masterDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        masterDetail.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var groupHoaDon = new GroupBox
        {
            Text = "Danh sách hóa đơn", Dock = DockStyle.Fill, Padding = new Padding(8, 5, 8, 4)
        };
        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 49));
        gridHoaDon = MakeGrid("gridHoaDon");
        AddColumn(gridHoaDon, "SoHoaDon", "Mã HD", 83);
        AddColumn(gridHoaDon, "NgayLap", "Ngày lập", 128, "dd/MM/yyyy HH:mm");
        AddColumn(gridHoaDon, "SoBan", "Số bàn", 65);
        AddColumn(gridHoaDon, "HinhThucTT", "Hình thức TT", 103);
        AddColumn(gridHoaDon, "TongTien", "Tổng tiền", 90, "N0", true);
        lblDoanhThu = new Label
        {
            Name = "lblDoanhThu", Text = "Tổng doanh thu đang hiển thị: 0 đ", Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 15, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft
        };
        left.Controls.Add(gridHoaDon, 0, 0);
        left.Controls.Add(lblDoanhThu, 0, 1);
        groupHoaDon.Controls.Add(left);
        masterDetail.Controls.Add(groupHoaDon, 0, 0);

        groupChiTiet = new GroupBox
        {
            Name = "groupChiTiet", Text = "Chi tiết hóa đơn", Dock = DockStyle.Fill,
            Padding = new Padding(8, 5, 8, 4)
        };
        var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Margin = new Padding(0) };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 49));

        var invoiceInputs = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        invoiceInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        invoiceInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56));
        invoiceInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        invoiceInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        dtpNgayLap = MakeDate("dtpNgayLap", "dd/MM/yyyy HH:mm");
        dtpNgayLap.MinDate = new DateTime(1753, 1, 1);
        dtpNgayLap.Dock = DockStyle.Top;
        cboSoBan = MakeCombo("cboSoBan");
        for (int i = 1; i <= 10; i++) cboSoBan.Items.Add(i);
        cboSoBan.SelectedIndex = 0;
        var payments = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0, 3, 0, 0)
        };
        rdoTienMat = new RadioButton
        {
            Name = "rdoTienMat", Text = "Tiền mặt", AutoSize = true, Checked = true, Margin = new Padding(1, 0, 3, 0)
        };
        rdoChuyenKhoan = new RadioButton
        {
            Name = "rdoChuyenKhoan", Text = "Chuyển khoản", AutoSize = true, Margin = new Padding(0)
        };
        payments.Controls.AddRange([rdoTienMat, rdoChuyenKhoan]);
        invoiceInputs.Controls.Add(dtpNgayLap, 0, 0);
        invoiceInputs.Controls.Add(MakeLabel("Số bàn:"), 1, 0);
        invoiceInputs.Controls.Add(cboSoBan, 2, 0);
        invoiceInputs.Controls.Add(payments, 3, 0);
        right.Controls.Add(invoiceInputs, 0, 0);

        var notes = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        notes.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        notes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        txtGhiChu = new TextBox
        {
            Name = "txtGhiChu", Dock = DockStyle.Fill, Multiline = true,
            MaxLength = 200, ScrollBars = ScrollBars.Vertical
        };
        notes.Controls.Add(MakeLabel("Ghi chú:"), 0, 0);
        notes.Controls.Add(txtGhiChu, 1, 0);
        right.Controls.Add(notes, 0, 1);

        var items = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1, Padding = new Padding(0, 6, 0, 0)
        };
        items.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        items.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        items.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28));
        items.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        items.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 99));
        cboMon = MakeCombo("cboMon");
        cboMon.DropDownWidth = 350;
        nudSoLuong = new NumericUpDown
        {
            Name = "nudSoLuong", Minimum = 1, Maximum = 50, Value = 1, Dock = DockStyle.Top
        };
        btnThemMon = MakeButton("btnThemMon", "Thêm món");
        items.Controls.Add(MakeLabel("Chọn món:"), 0, 0);
        items.Controls.Add(cboMon, 1, 0);
        items.Controls.Add(MakeLabel("SL:"), 2, 0);
        items.Controls.Add(nudSoLuong, 3, 0);
        items.Controls.Add(btnThemMon, 4, 0);
        right.Controls.Add(items, 0, 2);

        gridChiTiet = MakeGrid("gridChiTiet");
        AddColumn(gridChiTiet, "STT", "STT", 42);
        AddColumn(gridChiTiet, "TenMon", "Tên món", 165);
        AddColumn(gridChiTiet, "SoLuong", "SL", 58);
        AddColumn(gridChiTiet, "DonGia", "Đơn giá", 102, "N0", true);
        AddColumn(gridChiTiet, "ThanhTien", "Thành tiền", 110, "N0", true);
        right.Controls.Add(gridChiTiet, 0, 3);
        lblTongCong = new Label
        {
            Name = "lblTongCong", Text = "Tổng cộng: 0 đ", Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 17, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight
        };
        right.Controls.Add(lblTongCong, 0, 4);
        groupChiTiet.Controls.Add(right);
        masterDetail.Controls.Add(groupChiTiet, 1, 0);
        contentPanel.Controls.Add(masterDetail, 0, 1);

        var actions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 8, RowCount = 1, Margin = new Padding(0, 5, 0, 0) };
        for (int i = 0; i < 8; i++) actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
        btnHoaDonMoi = MakeButton("btnHoaDonMoi", "Hóa đơn mới");
        btnSuaHoaDon = MakeButton("btnSuaHoaDon", "Sửa hóa đơn");
        btnXoaHoaDon = MakeButton("btnXoaHoaDon", "Xóa hóa đơn");
        btnSuaSL = MakeButton("btnSuaSL", "Sửa SL");
        btnXoaMon = MakeButton("btnXoaMon", "Xóa món");
        btnXem = MakeButton("btnXem", "Xem hóa đơn");
        btnPdf = MakeButton("btnPdf", "Xuất PDF");
        btnIn = MakeButton("btnIn", "In");
        Control[] buttons = [btnHoaDonMoi, btnSuaHoaDon, btnXoaHoaDon, btnSuaSL, btnXoaMon, btnXem, btnPdf, btnIn];
        for (int i = 0; i < buttons.Length; i++) actions.Controls.Add(buttons[i], i, 0);
        contentPanel.Controls.Add(actions, 0, 2);

        // Trạng thái nội bộ; ảnh minh họa không có thanh trạng thái hay nút Làm mới.
        lblStatus = new Label { Name = "lblStatus", Visible = false };
        lblCount = new Label { Name = "lblCount", Visible = false };
        btnLamMoi = new Button { Name = "btnLamMoi", Visible = false };
        Controls.Add(contentPanel);
        Controls.Add(lblStatus);
        Controls.Add(lblCount);
        Controls.Add(btnLamMoi);
        ResumeLayout(false);
    }

    private static Label MakeLabel(string text) => new()
    {
        Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft, Padding = new Padding(0, 4, 0, 0)
    };

    private static DateTimePicker MakeDate(string name, string format) => new()
    {
        Name = name, Dock = DockStyle.Top, Format = DateTimePickerFormat.Custom, CustomFormat = format, Width = 145
    };

    private static ComboBox MakeCombo(string name) => new()
    {
        Name = name, Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList,
        IntegralHeight = false, MaxDropDownItems = 12
    };

    private static Button MakeButton(string name, string text) => new()
    {
        Name = name, Text = text, Dock = DockStyle.Fill, UseVisualStyleBackColor = true,
        Margin = new Padding(3, 0, 5, 3)
    };

    private static DataGridView MakeGrid(string name) => new()
    {
        Name = name, Dock = DockStyle.Fill, AutoGenerateColumns = false,
        AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true,
        MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = true, RowHeadersWidth = 32,
        BackgroundColor = SystemColors.AppWorkspace, BorderStyle = BorderStyle.FixedSingle,
        ColumnHeadersHeight = 27, RowTemplate = { Height = 25 }
    };

    private static void AddColumn(DataGridView grid, string property, string title, int width,
        string format = "", bool numeric = false)
    {
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = property, DataPropertyName = property, HeaderText = title, FillWeight = width,
            MinimumWidth = 35, SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = format, Alignment = numeric ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft
            }
        });
    }
}
