using System;
using System.Collections.Generic;

namespace Cau2.Models;

public partial class Mon
{
    public int MaMon { get; set; }

    public string TenMon { get; set; } = null!;

    public string NhomMon { get; set; } = null!;

    public decimal DonGia { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}
