namespace Cau2.ViewModels;

// List<ChiTietIn> là nguồn Business Object của FastReport.
public sealed class ChiTietIn
{
    public int MaCT { get; set; }
    public int MaMon { get; set; }
    public int STT { get; set; }
    public string TenMon { get; set; } = "";
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien => SoLuong * DonGia;
}
