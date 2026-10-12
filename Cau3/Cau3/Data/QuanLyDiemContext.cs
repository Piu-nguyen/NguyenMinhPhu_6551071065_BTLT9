using System;
using System.Collections.Generic;
using Cau3.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau3.Data;

public partial class QuanLyDiemContext : DbContext
{
    public QuanLyDiemContext(DbContextOptions<QuanLyDiemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DiemMon> DiemMons { get; set; }

    public virtual DbSet<Lop> Lops { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_100_CI_AS");

        modelBuilder.Entity<DiemMon>(entity =>
        {
            entity.HasKey(e => e.MaDiem);

            entity.ToTable("DiemMon");

            entity.HasIndex(e => e.MaSV, "IX_DiemMon_MaSV");

            entity.Property(e => e.Diem).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.MaSV)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenMon).HasMaxLength(100);

            entity.HasOne(d => d.MaSVNavigation).WithMany(p => p.DiemMons)
                .HasForeignKey(d => d.MaSV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiemMon_SinhVien");
        });

        modelBuilder.Entity<Lop>(entity =>
        {
            entity.HasKey(e => e.MaLop);

            entity.ToTable("Lop");

            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenLop).HasMaxLength(100);
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSV);

            entity.ToTable("SinhVien");

            entity.HasIndex(e => e.MaLop, "IX_SinhVien_MaLop");

            entity.Property(e => e.MaSV)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(3);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SinhVien_Lop");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
