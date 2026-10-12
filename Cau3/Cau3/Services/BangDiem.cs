using Cau3.Models;

namespace Cau3.Services;

public sealed record KetQuaDiem(int TongTinChi, decimal? DiemTrungBinh, string XepLoai);

public static class BangDiem
{
    public static KetQuaDiem Tinh(IEnumerable<DiemMon> diem)
    {
        var rows = diem.ToList();
        int credits = rows.Sum(x => x.SoTinChi);
        if (credits == 0) return new(0, null, "Chưa có điểm");
        decimal average = Math.Round(rows.Sum(x => x.Diem * x.SoTinChi) / credits, 2,
            MidpointRounding.AwayFromZero);
        return new(credits, average, average >= 8 ? "Giỏi" : average >= 7 ? "Khá" : average >= 5 ? "Trung bình" : "Yếu");
    }
}
