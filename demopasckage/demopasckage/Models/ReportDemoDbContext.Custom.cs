using Microsoft.EntityFrameworkCore;

namespace Tao_Xuat_Report_Chap9.Models;

// Phần bổ sung cho DbContext do EF Core Power Tools sinh ra (file kia là auto-generated, sẽ bị ghi đè khi generate lại)
public partial class ReportDemoDbContext
{
    public ReportDemoDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(AppConfig.ConnectionString);
        }
    }
}