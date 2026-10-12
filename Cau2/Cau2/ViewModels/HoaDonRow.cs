namespace Cau2.ViewModels;

public sealed class HoaDonRow
{
    public int MaHD { get; set; }
    public string SoHoaDon => $"HD{MaHD:000000}";
    public DateTime NgayLap { get; set; }
    public int SoBan { get; set; }
    public string HinhThucTT { get; set; } = "";
    public string? GhiChu { get; set; }
    public decimal TongTien { get; set; }
}
