using System;
using System.Collections.Generic;

namespace Cau3.Models;

public partial class DiemMon
{
    public int MaDiem { get; set; }

    public string MaSV { get; set; } = null!;

    public string TenMon { get; set; } = null!;

    public int SoTinChi { get; set; }

    public decimal Diem { get; set; }

    public virtual SinhVien MaSVNavigation { get; set; } = null!;
}
