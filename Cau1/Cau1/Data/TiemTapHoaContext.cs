using System;
using System.Collections.Generic;
using Cau1.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau1.Data;

public partial class TiemTapHoaContext : DbContext
{
    public TiemTapHoaContext(DbContextOptions<TiemTapHoaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.MaHang);

            entity.ToTable("HangHoa");

            entity.Property(e => e.ConKinhDoanh).HasDefaultValue(true, "DF_HangHoa_ConKinhDoanh");
            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LoaiHang).HasMaxLength(50);
            entity.Property(e => e.NgayNhap)
                .HasDefaultValueSql("(getdate())", "DF_HangHoa_NgayNhap")
                .HasColumnType("datetime");
            entity.Property(e => e.TenHang).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
