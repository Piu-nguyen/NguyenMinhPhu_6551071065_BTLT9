using System;
using System.Collections.Generic;

namespace Cau2.Models;

public partial class ChiTietHoaDon
{
    public int MaCT { get; set; }

    public int MaHD { get; set; }

    public int MaMon { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual HoaDon MaHDNavigation { get; set; } = null!;

    public virtual Mon MaMonNavigation { get; set; } = null!;
}
