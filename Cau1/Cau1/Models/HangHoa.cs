using System;
using System.Collections.Generic;

namespace Cau1.Models;

public partial class HangHoa
{
    public int MaHang { get; set; }

    public string TenHang { get; set; } = null!;

    public string LoaiHang { get; set; } = null!;

    public string DonVi { get; set; } = null!;

    public decimal GiaBan { get; set; }

    public int SoLuongTon { get; set; }

    public DateTime NgayNhap { get; set; }

    public bool ConKinhDoanh { get; set; }
}
