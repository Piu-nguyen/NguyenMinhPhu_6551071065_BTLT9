using System;
using System.Collections.Generic;
using Cau2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau2.Data;

public partial class QuanCaPheContext : DbContext
{
    public QuanCaPheContext(DbContextOptions<QuanCaPheContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<Mon> Mons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaCT);

            entity.ToTable("ChiTietHoaDon");

            entity.HasIndex(e => e.MaHD, "IX_ChiTietHoaDon_MaHD");

            entity.HasIndex(e => e.MaMon, "IX_ChiTietHoaDon_MaMon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaHDNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHD)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHoaDon_HoaDon");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHoaDon_Mon");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHD);

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.NgayLap, "IX_HoaDon_NgayLap");

            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.HinhThucTT).HasMaxLength(20);
            entity.Property(e => e.NgayLap).HasColumnType("datetime");
        });

        modelBuilder.Entity<Mon>(entity =>
        {
            entity.HasKey(e => e.MaMon);

            entity.ToTable("Mon");

            entity.Property(e => e.MaMon).ValueGeneratedNever();
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NhomMon).HasMaxLength(30);
            entity.Property(e => e.TenMon).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
