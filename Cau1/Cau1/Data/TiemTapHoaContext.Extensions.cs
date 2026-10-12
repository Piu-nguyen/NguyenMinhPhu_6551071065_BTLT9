using Microsoft.EntityFrameworkCore;

namespace Cau1.Data;

public partial class TiemTapHoaContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // false phải được ghi vào CSDL, không bị thay bằng DEFAULT (1).
        modelBuilder.Entity<Models.HangHoa>()
            .Property(x => x.ConKinhDoanh).HasSentinel(true);
    }
}
