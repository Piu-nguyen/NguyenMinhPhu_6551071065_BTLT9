using System;
using System.Collections.Generic;

namespace Cau3.Models;

public partial class Lop
{
    public string MaLop { get; set; } = null!;

    public string TenLop { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
