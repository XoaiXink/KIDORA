namespace KIDORA.Data
{
    public partial class Admin
    {
        public string MaAdmin { get; set; } = null!;
        public string? CuaHang { get; set; }
        public string? Quyen { get; set; }   // 'SUPER_ADMIN', 'STAFF', ...

        public virtual NguoiDung MaAdminNavigation { get; set; } = null!;
    }
}
