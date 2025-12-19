using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Data;

public partial class KidoraDbContext : DbContext
{
    public KidoraDbContext()
    {
    }

    public KidoraDbContext(DbContextOptions<KidoraDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AnhSanPham> AnhSanPhams { get; set; }

    public virtual DbSet<BienTheGium> BienTheGia { get; set; }

    public virtual DbSet<BienTheHinh> BienTheHinhs { get; set; }

    public virtual DbSet<BienTheSanPham> BienTheSanPhams { get; set; }

    public virtual DbSet<BienTheTonKho> BienTheTonKhos { get; set; }

    public virtual DbSet<CamNang> CamNangs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<DanhGiaSanPham> DanhGiaSanPhams { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DiaChiKhachHang> DiaChiKhachHangs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonViVanChuyen> DonViVanChuyens { get; set; }

    public virtual DbSet<GdKcoin> GdKcoins { get; set; }

    public virtual DbSet<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HangThanhVien> HangThanhViens { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KmDonHang> KmDonHangs { get; set; }

    public virtual DbSet<MaKhuyenMai> MaKhuyenMais { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<ThuocTinh> ThuocTinhs { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<WishlistChiTiet> WishlistChiTiets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=KidoraDB;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.MaAdmin).HasName("PK__ADMIN__49341E382F1B29E6");

            entity.ToTable("ADMIN");

            entity.Property(e => e.MaAdmin)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CuaHang).HasMaxLength(200);
            entity.Property(e => e.Quyen).HasMaxLength(50);

            entity.HasOne(d => d.MaAdminNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.MaAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ADMIN__MaAdmin__56E8E7AB");
        });

        modelBuilder.Entity<AnhSanPham>(entity =>
        {
            entity.HasKey(e => e.MaAnh).HasName("PK__ANH_SAN___356240DF29137B3B");

            entity.ToTable("ANH_SAN_PHAM");

            entity.Property(e => e.MaAnh).ValueGeneratedNever();
            entity.Property(e => e.DuongDanAnh)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.AnhSanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ANH_SAN_PH__MaSP__6442E2C9");
        });

        modelBuilder.Entity<BienTheGium>(entity =>
        {
            entity.HasKey(e => e.MaBienThe).HasName("PK__BIEN_THE__3987CEF57EAC3E31");

            entity.ToTable("BIEN_THE_GIA");

            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GiaBan).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.GiaNiemYet).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

            entity.HasOne(d => d.MaBienTheNavigation).WithOne(p => p.BienTheGium)
                .HasForeignKey<BienTheGium>(d => d.MaBienThe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BIEN_THE___MaBie__0880433F");
        });

        modelBuilder.Entity<BienTheHinh>(entity =>
        {
            entity.HasKey(e => e.MaAnhBienThe).HasName("PK__BIEN_THE__9F2C1B8DA37CA814");

            entity.ToTable("BIEN_THE_HINH");

            entity.Property(e => e.MaAnhBienThe).ValueGeneratedNever();
            entity.Property(e => e.DuongDanAnh)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.BienTheHinhs)
                .HasForeignKey(d => d.MaBienThe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BIEN_THE___MaBie__0B5CAFEA");
        });

        modelBuilder.Entity<BienTheSanPham>(entity =>
        {
            entity.HasKey(e => e.MaBienThe).HasName("PK__BIEN_THE__3987CEF57FD26D83");

            entity.ToTable("BIEN_THE_SAN_PHAM");

            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.TenBienThe).HasMaxLength(200);

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.BienTheSanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BIEN_THE_S__MaSP__7C1A6C5A");

            entity.HasMany(d => d.MaGiaTris).WithMany(p => p.MaBienThes)
                .UsingEntity<Dictionary<string, object>>(
                    "SpThuocTinhGiaTri",
                    r => r.HasOne<GiaTriThuocTinh>().WithMany()
                        .HasForeignKey("MaGiaTri")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SP_THUOC___MaGia__7FEAFD3E"),
                    l => l.HasOne<BienTheSanPham>().WithMany()
                        .HasForeignKey("MaBienThe")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SP_THUOC___MaBie__7EF6D905"),
                    j =>
                    {
                        j.HasKey("MaBienThe", "MaGiaTri").HasName("PK__SP_THUOC__9412175AB7B5F262");
                        j.ToTable("SP_THUOC_TINH_GIA_TRI");
                        j.IndexerProperty<string>("MaBienThe")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<BienTheTonKho>(entity =>
        {
            entity.HasKey(e => e.MaBienThe).HasName("PK__BIEN_THE__3987CEF5FDEB8E56");

            entity.ToTable("BIEN_THE_TON_KHO");

            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

            entity.HasOne(d => d.MaBienTheNavigation).WithOne(p => p.BienTheTonKho)
                .HasForeignKey<BienTheTonKho>(d => d.MaBienThe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BIEN_THE___MaBie__05A3D694");
        });

        modelBuilder.Entity<CamNang>(entity =>
        {
            entity.HasKey(e => e.MaBai).HasName("PK__CAM_NANG__3520ED77F50AF1A5");

            entity.ToTable("CAM_NANG");

            entity.Property(e => e.MaBai)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.TacGia).HasMaxLength(100);
            entity.Property(e => e.TieuDe).HasMaxLength(255);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaCtdh).HasName("PK__CHI_TIET__1E4E40F086373CE3");

            entity.ToTable("CHI_TIET_DON_HANG");

            entity.Property(e => e.MaCtdh)
                .ValueGeneratedNever()
                .HasColumnName("MaCTDH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.TenSpHienThi)
                .HasMaxLength(200)
                .HasColumnName("TenSP_HienThi");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TyLeGiam).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaBienThe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_TIET___MaBie__10216507");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_TIET___MaDon__0F2D40CE");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.MaCtgh).HasName("PK__CHI_TIET__1E4FAF54E9DD73BF");

            entity.ToTable("CHI_TIET_GIO_HANG");

            entity.Property(e => e.MaCtgh).HasColumnName("MaCTGH");
            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaGioHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaBienThe)
                .HasConstraintName("FK__CHI_TIET___MaBie__2610A626");

            entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaGioHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_TIET___MaGio__24285DB4");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_TIET_G__MaSP__251C81ED");
        });

        modelBuilder.Entity<DanhGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DANH_GIA__AA9515BF370E03EC");

            entity.ToTable("DANH_GIA_SAN_PHAM");

            entity.Property(e => e.MaDanhGia).ValueGeneratedNever();
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.NgayDanhGia).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DANH_GIA_S__MaKH__1A9EF37A");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DANH_GIA_S__MaSP__19AACF41");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DANH_MUC__B375088754D075BB");

            entity.ToTable("DANH_MUC");

            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AnhDanhMuc)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaDanhMucCha)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(200);

            entity.HasOne(d => d.MaDanhMucChaNavigation).WithMany(p => p.InverseMaDanhMucChaNavigation)
                .HasForeignKey(d => d.MaDanhMucCha)
                .HasConstraintName("FK__DANH_MUC__MaDanh__498EEC8D");
        });

        modelBuilder.Entity<DiaChiKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaDiaChi).HasName("PK__DIA_CHI___EB61213E1E5751FE");

            entity.ToTable("DIA_CHI_KHACH_HANG");

            entity.Property(e => e.DiaChiChiTiet).HasMaxLength(250);
            entity.Property(e => e.DienThoaiNhan)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.PhuongXa).HasMaxLength(100);
            entity.Property(e => e.QuanHuyen).HasMaxLength(100);
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.TinhThanh).HasMaxLength(100);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DiaChiKhachHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DIA_CHI_KH__MaKH__59C55456");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DON_HANG__129584AD326AC6DA");

            entity.ToTable("DON_HANG");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiaChiGiao).HasMaxLength(250);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.GiamGiaKcoin)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("GiamGiaKCoin");
            entity.Property(e => e.GiamGiaKm)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("GiamGiaKM");
            entity.Property(e => e.MaDvvc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaDVVC");
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.NgayDat).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.SdtnguoiNhan)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDTNguoiNhan");
            entity.Property(e => e.SoDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.TongGiamGia).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TongSauVat)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("TongSauVAT");
            entity.Property(e => e.TongThanhToan).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TongTienHang).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TrangThaiDon).HasMaxLength(50);
            entity.Property(e => e.TrangThaiThanhToan).HasMaxLength(50);
            entity.Property(e => e.Vat)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("VAT");

            entity.HasOne(d => d.MaDvvcNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaDvvc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DON_HANG__MaDVVC__74794A92");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DON_HANG__MaKH__73852659");
        });

        modelBuilder.Entity<DonViVanChuyen>(entity =>
        {
            entity.HasKey(e => e.MaDvvc).HasName("PK__DON_VI_V__36ECC45E3407AE5A");

            entity.ToTable("DON_VI_VAN_CHUYEN");

            entity.Property(e => e.MaDvvc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaDVVC");
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenDvvc)
                .HasMaxLength(200)
                .HasColumnName("TenDVVC");
            entity.Property(e => e.TrackingUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("TrackingURL");
        });

        modelBuilder.Entity<GdKcoin>(entity =>
        {
            entity.HasKey(e => e.MaGdKcoin).HasName("PK__GD_KCOIN__41B15EDCCFD8061A");

            entity.ToTable("GD_KCOIN");

            entity.Property(e => e.MaGdKcoin)
                .ValueGeneratedNever()
                .HasColumnName("MaGD_KCoin");
            entity.Property(e => e.KieuGd)
                .HasMaxLength(20)
                .HasColumnName("KieuGD");
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.SoDuSauGd).HasColumnName("SoDuSauGD");
            entity.Property(e => e.SoKcoin).HasColumnName("SoKCoin");
            entity.Property(e => e.ThoiGianGd)
                .HasColumnType("datetime")
                .HasColumnName("ThoiGianGD");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.GdKcoins)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__GD_KCOIN__MaDonH__16CE6296");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.GdKcoins)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GD_KCOIN__MaKH__15DA3E5D");
        });

        modelBuilder.Entity<GiaTriThuocTinh>(entity =>
        {
            entity.HasKey(e => e.MaGiaTri).HasName("PK__GIA_TRI___D95D9AFB43FE3C40");

            entity.ToTable("GIA_TRI_THUOC_TINH");

            entity.Property(e => e.MaGiaTri).ValueGeneratedNever();
            entity.Property(e => e.GiaTri).HasMaxLength(100);
            entity.Property(e => e.GiaTriHienThi).HasMaxLength(100);

            entity.HasOne(d => d.MaThuocTinhNavigation).WithMany(p => p.GiaTriThuocTinhs)
                .HasForeignKey(d => d.MaThuocTinh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GIA_TRI_T__MaThu__793DFFAF");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GIO_HANG__F5001DA3B298B998");

            entity.ToTable("GIO_HANG");

            entity.Property(e => e.MaGioHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GIO_HANG__MaKH__214BF109");
        });

        modelBuilder.Entity<HangThanhVien>(entity =>
        {
            entity.HasKey(e => e.MaHang).HasName("PK__HANG_THA__19C0DB1D891F197D");

            entity.ToTable("HANG_THANH_VIEN");

            entity.Property(e => e.MaHang)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.MucChiTieu6Thang).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TenHang).HasMaxLength(50);
            entity.Property(e => e.TyLeHoanKcoin)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("TyLeHoanKCoin");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACH_HA__2725CF1E88EDF78A");

            entity.ToTable("KHACH_HANG");

            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaHang)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayThamGia).HasColumnType("datetime");
            entity.Property(e => e.SoDuKcoin).HasColumnName("SoDuKCoin");

            entity.HasOne(d => d.MaHangNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KHACH_HAN__MaHan__540C7B00");

            entity.HasOne(d => d.MaKhNavigation).WithOne(p => p.KhachHang)
                .HasForeignKey<KhachHang>(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KHACH_HANG__MaKH__531856C7");
        });

        modelBuilder.Entity<KmDonHang>(entity =>
        {
            entity.HasKey(e => e.MaKmDon).HasName("PK__KM_DON_H__C4EB1C5736BD2306");

            entity.ToTable("KM_DON_HANG");

            entity.Property(e => e.MaKmDon)
                .ValueGeneratedNever()
                .HasColumnName("MaKM_Don");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKm)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKM");
            entity.Property(e => e.SoTienGiam).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.KmDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KM_DON_HA__MaDon__1D7B6025");

            entity.HasOne(d => d.MaKmNavigation).WithMany(p => p.KmDonHangs)
                .HasForeignKey(d => d.MaKm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KM_DON_HAN__MaKM__1E6F845E");
        });

        modelBuilder.Entity<MaKhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__MA_KHUYE__2725CF15D0DAAA57");

            entity.ToTable("MA_KHUYEN_MAI");

            entity.Property(e => e.MaKm)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKM");
            entity.Property(e => e.DonToiThieu).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.KieuGiam).HasMaxLength(20);
            entity.Property(e => e.MaCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenKm)
                .HasMaxLength(200)
                .HasColumnName("TenKM");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NGUOI_DU__3214EC07B3E94990");

            entity.ToTable("NGUOI_DUNG");

            entity.HasIndex(e => e.Email, "UQ__NGUOI_DU__A9D10534B1D9F609").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(3);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.LoaiNguoiDung)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MatKhauHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResetToken)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ResetTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.VerificationToken)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PK__NHA_CUNG__3A185DEB7A3BD3BB");

            entity.ToTable("NHA_CUNG_CAP");

            entity.Property(e => e.MaNcc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HinhThucThanhToan).HasMaxLength(100);
            entity.Property(e => e.QuocGia).HasMaxLength(100);
            entity.Property(e => e.TenLienHe).HasMaxLength(100);
            entity.Property(e => e.TenNcc)
                .HasMaxLength(200)
                .HasColumnName("TenNCC");
            entity.Property(e => e.ThanhPho).HasMaxLength(100);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SAN_PHAM__2725081C7F51BF54");

            entity.ToTable("SAN_PHAM");

            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.AnhChinh)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DangBan).HasDefaultValue(true);
            entity.Property(e => e.DonGiaBan).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.GiaNiemYet).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.KhoiLuong).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNcc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNCC");
            entity.Property(e => e.MoTaNgan).HasMaxLength(500);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.TenSp)
                .HasMaxLength(200)
                .HasColumnName("TenSP");
            entity.Property(e => e.ThuongHieu).HasMaxLength(100);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SAN_PHAM__MaDanh__607251E5");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SAN_PHAM__MaNCC__6166761E");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaTt).HasName("PK__THANH_TO__2725007963204950");

            entity.ToTable("THANH_TOAN");

            entity.Property(e => e.MaTt)
                .ValueGeneratedNever()
                .HasColumnName("MaTT");
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaGiaoDich)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.PhuongThuc).HasMaxLength(50);
            entity.Property(e => e.SoTienThanhToan).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__THANH_TOA__MaDon__12FDD1B2");
        });

        modelBuilder.Entity<ThuocTinh>(entity =>
        {
            entity.HasKey(e => e.MaThuocTinh).HasName("PK__THUOC_TI__9EA5FC478FB0B8B8");

            entity.ToTable("THUOC_TINH");

            entity.Property(e => e.MaThuocTinh).ValueGeneratedNever();
            entity.Property(e => e.KieuDuLieu).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenThuocTinh).HasMaxLength(100);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.MaWishlist).HasName("PK__WISHLIST__B290E878E8E2D9D5");

            entity.ToTable("WISHLIST");

            entity.Property(e => e.MaWishlist)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WISHLIST__MaKH__28ED12D1");
        });

        modelBuilder.Entity<WishlistChiTiet>(entity =>
        {
            entity.HasKey(e => new { e.MaWishlist, e.MaBienThe }).HasName("PK__WISHLIST__F1089497D87B2003");

            entity.ToTable("WISHLIST_CHI_TIET");

            entity.Property(e => e.MaWishlist)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaBienThe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSp)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.WishlistChiTiets)
                .HasForeignKey(d => d.MaBienThe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WISHLIST___MaBie__2DB1C7EE");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.WishlistChiTiets)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WISHLIST_C__MaSP__2CBDA3B5");

            entity.HasOne(d => d.MaWishlistNavigation).WithMany(p => p.WishlistChiTiets)
                .HasForeignKey(d => d.MaWishlist)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WISHLIST___MaWis__2BC97F7C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
