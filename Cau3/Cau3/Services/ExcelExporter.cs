using ClosedXML.Excel;

namespace Cau3.Services;

public sealed record DiemExcel(string MaSV, string HoTen, string MaLop, string TenMon, int SoTinChi, decimal Diem);

public static class ExcelExporter
{
    public static void Save(string path, IReadOnlyList<DiemExcel> rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Điểm lớp");
        string[] headers = ["MSSV", "Họ tên", "Lớp", "Môn học", "Số TC", "Điểm"];
        for (int col = 0; col < headers.Length; col++) sheet.Cell(1, col + 1).Value = headers[col];
        for (int i = 0; i < rows.Count; i++)
        {
            var item = rows[i];
            int r = i + 2;
            sheet.Cell(r, 1).Value = item.MaSV;
            sheet.Cell(r, 2).Value = item.HoTen;
            sheet.Cell(r, 3).Value = item.MaLop;
            sheet.Cell(r, 4).Value = item.TenMon;
            sheet.Cell(r, 5).Value = item.SoTinChi;
            sheet.Cell(r, 6).Value = item.Diem;
        }
        int last = rows.Count + 1;
        sheet.Range(1, 1, last, 6).SetAutoFilter();
        sheet.SheetView.FreezeRows(1);
        sheet.Range(1, 1, 1, 6).Style.Font.Bold = true;
        sheet.Range(1, 1, 1, 6).Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
        sheet.Range(1, 1, last, 6).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        sheet.Range(1, 1, last, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        sheet.Range(2, 5, last + 2, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        sheet.Column(5).Style.NumberFormat.Format = "0";
        sheet.Column(6).Style.NumberFormat.Format = "0.00";
        sheet.Cell(last + 1, 4).Value = "Điểm trung bình";
        sheet.Cell(last + 1, 6).FormulaA1 = $"AVERAGE(F2:F{last})";
        sheet.Cell(last + 2, 4).Value = "Tổng số tín chỉ";
        sheet.Cell(last + 2, 5).FormulaA1 = $"SUM(E2:E{last})";
        sheet.Range(last + 1, 4, last + 2, 6).Style.Font.Bold = true;
        sheet.Columns().AdjustToContents();
        sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        sheet.PageSetup.FitToPages(1, 0);
        workbook.SaveAs(path);
    }
}
