namespace Cau3;

public partial class Form1
{
    private readonly ListBox lstLop = new() { Dock = DockStyle.Fill, HorizontalScrollbar = true, Font = new Font("Segoe UI", 9F) };
    private readonly TextBox txtTim = new() { Dock = DockStyle.Fill, PlaceholderText = "Tìm MSSV / họ tên" };
    private readonly DataGridView dgvSinhVien = CreateGrid();
    private readonly DataGridView dgvDiem = CreateGrid();
    private readonly TextBox txtMaSV = new() { MaxLength = 10, Dock = DockStyle.Fill };
    private readonly TextBox txtHoTen = new() { MaxLength = 100, Dock = DockStyle.Fill };
    private readonly DateTimePicker dtpNgaySinh = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Dock = DockStyle.Fill, Value = new DateTime(2005, 1, 1) };
    private readonly RadioButton radNam = new() { Text = "Nam", AutoSize = true };
    private readonly RadioButton radNu = new() { Text = "Nữ", AutoSize = true, Checked = true };
    private readonly ComboBox cboLop = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private readonly TextBox txtTenMon = new() { MaxLength = 100, Dock = DockStyle.Fill };
    private readonly NumericUpDown nudTinChi = new() { Minimum = 1, Maximum = 6, Value = 3, Dock = DockStyle.Fill };
    private readonly NumericUpDown nudDiem = new() { Minimum = 0, Maximum = 10, DecimalPlaces = 1, Increment = 0.1M, Dock = DockStyle.Fill };
    private readonly Label lblTinChi = SummaryLabel("Tổng tín chỉ: 0");
    private readonly Label lblTrungBinh = SummaryLabel("Điểm TB: —");
    private readonly Label lblXepLoai = SummaryLabel("Chưa có điểm");
    private readonly Label lblTrangThai = new() { AutoSize = true, Text = "Đang tải dữ liệu...", Dock = DockStyle.Fill };
    private readonly GroupBox grpDiem = new() { Text = "Điểm của sinh viên", Dock = DockStyle.Fill, Padding = new Padding(10) };
    private readonly TableLayoutPanel mainLayout = new() { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 4 };

    private static DataGridView CreateGrid() => new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AllowUserToDeleteRows = false, AutoGenerateColumns = false, MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.DarkGray, RowHeadersWidth = 28, RowTemplate = { Height = 29 },
        ColumnHeadersHeight = 32, AllowUserToResizeRows = false
    };

    private static Label SummaryLabel(string text) => new() { Text = text, AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Margin = new Padding(4, 10, 12, 0) };
    private static Label Caption(string text) => new() { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 3, 6) };
    private static FlowLayoutPanel Flow() => new() { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
    private Button ActionButton(string text, Func<Task> action, bool busy = true)
    {
        var button = new Button { Text = text, AutoSize = true, MinimumSize = new Size(85, 34), Margin = new Padding(3, 5, 5, 3) };
        button.Click += async (_, _) => await RunAsync(action, busy);
        return button;
    }

    private static void AddColumn(DataGridView grid, string property, string title, float weight, string? format = null)
    {
        var column = new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = title, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable };
        if (format != null) column.DefaultCellStyle.Format = format;
        grid.Columns.Add(column);
    }

    private void BuildLayout()
    {
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        Controls.Add(mainLayout);
        var top = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 51));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
        mainLayout.Controls.Add(top, 0, 0);
        var classes = new GroupBox { Text = "Lớp", Dock = DockStyle.Fill, Padding = new Padding(10) };
        classes.Controls.Add(lstLop);
        top.Controls.Add(classes, 0, 0);
        var students = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(4, 0, 4, 0) };
        students.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        students.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        students.Controls.Add(txtTim, 0, 0);
        students.Controls.Add(dgvSinhVien, 0, 1);
        top.Controls.Add(students, 1, 0);
        var info = new GroupBox { Text = "Thông tin sinh viên", Dock = DockStyle.Fill, Padding = new Padding(10) };
        var inputs = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7 };
        inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 5; i++) inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        inputs.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        inputs.Controls.Add(Caption("MSSV"), 0, 0); inputs.Controls.Add(txtMaSV, 1, 0);
        inputs.Controls.Add(Caption("Họ tên"), 0, 1); inputs.Controls.Add(txtHoTen, 1, 1);
        inputs.Controls.Add(Caption("Ngày sinh"), 0, 2); inputs.Controls.Add(dtpNgaySinh, 1, 2);
        var gender = Flow(); gender.Controls.AddRange([radNam, radNu]);
        inputs.Controls.Add(gender, 1, 3);
        inputs.Controls.Add(Caption("Lớp"), 0, 4); inputs.Controls.Add(cboLop, 1, 4);
        var studentButtons = Flow();
        studentButtons.Controls.AddRange([ActionButton("Thêm", AddStudentAsync), ActionButton("Sửa", EditStudentAsync), ActionButton("Xóa", DeleteStudentAsync)]);
        inputs.Controls.Add(studentButtons, 0, 5); inputs.SetColumnSpan(studentButtons, 2);
        var clear = ActionButton("Làm mới", () => { ClearStudent(); return Task.CompletedTask; });
        inputs.Controls.Add(clear, 0, 6); inputs.SetColumnSpan(clear, 2);
        info.Controls.Add(inputs); top.Controls.Add(info, 2, 0);

        mainLayout.Controls.Add(grpDiem, 0, 1);
        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 51)); bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49));
        bottom.Controls.Add(dgvDiem, 0, 0);
        var gradeInputs = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10, 0, 0, 0) };
        gradeInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
        gradeInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        gradeInputs.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2 };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        fields.Controls.Add(Caption("Tên môn"), 0, 0); fields.Controls.Add(Caption("Số TC"), 1, 0); fields.Controls.Add(Caption("Điểm"), 2, 0);
        fields.Controls.Add(txtTenMon, 0, 1); fields.Controls.Add(nudTinChi, 1, 1); fields.Controls.Add(nudDiem, 2, 1);
        gradeInputs.Controls.Add(fields, 0, 0);
        var gradeButtons = Flow(); gradeButtons.Controls.AddRange([ActionButton("Thêm môn", AddGradeAsync), ActionButton("Sửa điểm", EditGradeAsync), ActionButton("Xóa môn", DeleteGradeAsync)]);
        gradeInputs.Controls.Add(gradeButtons, 0, 1);
        var summary = Flow(); summary.Controls.AddRange([lblTinChi, lblTrungBinh, lblXepLoai]); gradeInputs.Controls.Add(summary, 0, 2);
        bottom.Controls.Add(gradeInputs, 1, 0); grpDiem.Controls.Add(bottom);
        var printButtons = Flow();
        printButtons.Controls.AddRange([ActionButton("Xem trước phiếu", PreviewAsync), ActionButton("In...", PrintAsync), ActionButton("Cài đặt trang...", PageSetupAsync), ActionButton("Xuất Excel điểm lớp", ExportAsync)]);
        mainLayout.Controls.Add(printButtons, 0, 2); mainLayout.Controls.Add(lblTrangThai, 0, 3);

        AddColumn(dgvSinhVien, "MaSV", "MSSV", 16); AddColumn(dgvSinhVien, "HoTen", "Họ tên", 32);
        AddColumn(dgvSinhVien, "NgaySinh", "Ngày sinh", 20, "dd/MM/yyyy"); AddColumn(dgvSinhVien, "GioiTinh", "Giới tính", 18); AddColumn(dgvSinhVien, "MaLop", "Lớp", 14);
        AddColumn(dgvDiem, "STT", "STT", 10); AddColumn(dgvDiem, "TenMon", "Môn học", 62); AddColumn(dgvDiem, "SoTinChi", "Số TC", 14); AddColumn(dgvDiem, "Diem", "Điểm", 14, "N1");
    }
}
