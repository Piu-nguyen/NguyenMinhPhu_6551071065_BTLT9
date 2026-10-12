using ClosedXML.Excel;
using System.Data;
using Tao_Xuat_Report_Chap9.Helpers;

namespace Tao_Xuat_Report_Chap9.Services;

public sealed record CotExcel(string TieuDe, string? DinhDang = null, bool CongTong = false, string? CongThuc = null);

public static class ExcelExporter
{
    public static void Xuat(string filePath, string tenSheet, IReadOnlyList<CotExcel> cot, IEnumerable<object?[]> dong)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(tenSheet);
        int soCot = cot.Count;

        for (int c = 0; c < soCot; c++)
            ws.Cell(1, c + 1).Value = cot[c].TieuDe;

        var tieuDe = ws.Range(1, 1, 1, soCot);
        tieuDe.Style.Font.Bold = true;
        tieuDe.Style.Fill.BackgroundColor = XLColor.LightGray;
        tieuDe.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int r = 1;
        foreach (var d in dong)
        {
            r++;
            for (int c = 0; c < soCot; c++)
            {
                var cell = ws.Cell(r, c + 1);
                if (cot[c].CongThuc is { } ct)
                {
                    cell.FormulaA1 = ct.Replace("{r}", r.ToString()).TrimStart('=');
                }
                else if (c < d.Length && d[c] is { } v)
                {
                    cell.Value = ChuyenGiaTri(v);
                }
            }
        }

        int cuoi = r;

        if (cuoi >= 2)
        {
            for (int c = 0; c < soCot; c++)
                if (cot[c].DinhDang is { } fmt)
                    ws.Range(2, c + 1, cuoi, c + 1).Style.NumberFormat.Format = fmt;
        }

        if (cuoi >= 2 && cot.Any(x => x.CongTong))
        {
            int rt = cuoi + 1;
            ws.Cell(rt, 1).Value = "Tổng";
            for (int c = 0; c < soCot; c++)
            {
                if (!cot[c].CongTong) continue;
                string chu = XLHelper.GetColumnLetterFromNumber(c + 1);
                var cell = ws.Cell(rt, c + 1);
                cell.FormulaA1 = $"SUM({chu}2:{chu}{cuoi})";
                if (cot[c].DinhDang is { } fmt)
                    cell.Style.NumberFormat.Format = fmt;
            }
            ws.Range(rt, 1, rt, soCot).Style.Font.Bold = true;
        }

        if (cuoi >= 2)
            ws.Range(1, 1, cuoi, soCot).SetAutoFilter();
        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        wb.SaveAs(filePath);
    }

    public static void XuatDataGridView(DataGridView grid, string filePath, string tenSheet, params string[] cotCongTong)
    {
        var cols = grid.Columns.Cast<DataGridViewColumn>()
            .Where(c => c.Visible)
            .OrderBy(c => c.DisplayIndex)
            .ToList();

        var dong = grid.Rows.Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow)
            .Select(r => cols.Select(c => r.Cells[c.Index].Value).ToArray())
            .ToList();

        var cot = new List<CotExcel>();
        for (int i = 0; i < cols.Count; i++)
        {
            object? mau = dong.Select(d => d[i]).FirstOrDefault(v => v != null);
            cot.Add(new CotExcel(cols[i].HeaderText, DinhDangTheoKieu(mau), cotCongTong.Contains(cols[i].DataPropertyName)));
        }

        Xuat(filePath, tenSheet, cot, dong);
    }

    public static void XuatNhanhDataTable(DataTable bang, string filePath, string tenSheet)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(bang, tenSheet);
        ws.Columns().AdjustToContents();
        wb.SaveAs(filePath);
    }

    private static string? DinhDangTheoKieu(object? v) => v switch
    {
        decimal or double or float => "#,##0",
        DateTime => "dd/MM/yyyy HH:mm",
        _ => null
    };

    private static XLCellValue ChuyenGiaTri(object v)
    {
        switch (v)
        {
            case string s: return s;
            case bool b: return b;
            case DateTime dt: return dt;
            case TimeSpan ts: return ts;
            case decimal or double or float or byte or sbyte or short or ushort or int or uint or long or ulong:
                return Convert.ToDouble(v);
            default: return v.ToString() ?? "";
        }
    }
}