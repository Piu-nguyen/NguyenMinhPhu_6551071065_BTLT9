using FastReport;
using FastReport.Data;
using FastReport.Export.Pdf;
using System.Collections;
using System.Data;

namespace Tao_Xuat_Report_Chap9.Services;

public static class ReportService
{
    public static bool CoMau(string tenFile) => File.Exists(AppConfig.DuongDanReport(tenFile));

    public static Report TaoReport(string tenFile)
    {
        var path = AppConfig.DuongDanReport(tenFile);
        var report = new Report();

        if (File.Exists(path))
        {
            report.Load(path);
        }
        else
        {
            var page = new ReportPage();
            report.Pages.Add(page);
            page.CreateUniqueName();
        }

        report.FileName = path;
        report.DoublePass = true;
        return report;
    }

    public static void DangKy(Report report, IEnumerable duLieu, string ten)
    {
        report.RegisterData(duLieu, ten);
        report.GetDataSource(ten).Enabled = true;
    }

    public static void DangKy(Report report, DataTable bang, string ten)
    {
        report.RegisterData(bang, ten);
        report.GetDataSource(ten).Enabled = true;
    }

    public static void DatThamSo(Report report, string ten, object? giaTri)
    {
        var p = report.GetParameter(ten);
        if (p == null)
        {
            p = new Parameter { Name = ten };
            report.Dictionary.Parameters.Add(p);
        }
        p.Value = giaTri;
    }

    public static void XemTruoc(Report report)
    {
        if (report.Prepare())
            report.ShowPrepared(true);   // true = modal
    }

    public static void ThietKe(Report report) => report.Design(true);   // true = modal

    public static void XuatPdf(Report report, string filePath)
    {
        if (!report.Prepare()) return;
        using var pdf = new PDFExport();
        report.Export(pdf, filePath);
    }

    public static void InThang(Report report, bool hienHopThoai)
    {
        report.Prepare();
        report.PrintSettings.ShowDialog = hienHopThoai;
        report.Print();
    }
}
