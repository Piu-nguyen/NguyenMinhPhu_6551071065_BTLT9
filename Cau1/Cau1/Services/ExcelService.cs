using Cau1.Models;
using ClosedXML.Excel;

namespace Cau1.Services;

internal record GridExportColumn(int Index, string Title, string Property);

internal static class ExcelService
{
    public static void ExportGrid(string path, GridExportColumn[] columns, object?[][] rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Hàng hóa");
        for (int col = 0; col < columns.Length; col++)
        {
            sheet.Cell(1, col + 1).Value = columns[col].Title;
            for (int row = 0; row < rows.Length; row++)
                sheet.Cell(row + 2, col + 1).Value = XLCellValue.FromObject(rows[row][col]);
            if (columns[col].Property == "NgayNhap")
                sheet.Column(col + 1).Style.DateFormat.Format = "dd/MM/yyyy";
            else if (columns[col].Property is "GiaBan" or "SoLuongTon" or "MaHang")
                FormatNumber(sheet.Column(col + 1));
        }
        StyleSheet(sheet, rows.Length + 1, columns.Length);
        workbook.SaveAs(path);
    }

    public static void ExportInventory(string path, IReadOnlyList<HangHoa> items)
    {
        using var workbook = new XLWorkbook();
        workbook.CalculateMode = XLCalculateMode.Auto;
        var sheet = workbook.Worksheets.Add("Tồn kho");
        string[] headers = ["Mã hàng", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Số lượng tồn", "Ngày nhập", "Giá trị tồn"];
        for (int col = 0; col < headers.Length; col++) sheet.Cell(1, col + 1).Value = headers[col];
        for (int i = 0; i < items.Count; i++)
        {
            int row = i + 2;
            var item = items[i];
            sheet.Cell(row, 1).Value = item.MaHang;
            sheet.Cell(row, 2).Value = item.TenHang;
            sheet.Cell(row, 3).Value = item.LoaiHang;
            sheet.Cell(row, 4).Value = item.DonVi;
            sheet.Cell(row, 5).Value = item.GiaBan;
            sheet.Cell(row, 6).Value = item.SoLuongTon;
            sheet.Cell(row, 7).Value = item.NgayNhap;
            sheet.Cell(row, 8).FormulaA1 = $"E{row}*F{row}";
        }
        int lastDataRow = items.Count + 1;
        int totalRow = lastDataRow + 1;
        sheet.Range(totalRow, 1, totalRow, 7).Merge().Value = "Tổng";
        sheet.Cell(totalRow, 8).FormulaA1 = items.Count > 0 ? $"SUM(H2:H{lastDataRow})" : "SUM(0)";
        sheet.Range(totalRow, 1, totalRow, 8).Style.Font.Bold = true;
        sheet.Range(totalRow, 1, totalRow, 8).Style.Fill.BackgroundColor = XLColor.LightYellow;
        foreach (int col in new[] { 1, 5, 6, 8 }) FormatNumber(sheet.Column(col));
        sheet.Column(7).Style.DateFormat.Format = "dd/MM/yyyy";
        StyleSheet(sheet, lastDataRow, 8);
        sheet.Column(8).AdjustToContents();
        workbook.SaveAs(path, new SaveOptions { EvaluateFormulasBeforeSaving = true });
    }

    private static void FormatNumber(IXLColumn column)
    {
        column.Style.NumberFormat.Format = "#,##0";
        column.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }

    private static void StyleSheet(IXLWorksheet sheet, int lastRow, int columns)
    {
        var header = sheet.Range(1, 1, 1, columns);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");
        sheet.Range(1, 1, Math.Max(1, lastRow), columns).SetAutoFilter();
        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();
        sheet.Row(1).Height = 26;
        sheet.RangeUsed()!.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        sheet.RangeUsed()!.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }
}
