using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class Wishlist
{
    public string MaWishlist { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual ICollection<WishlistChiTiet> WishlistChiTiets { get; set; } = new List<WishlistChiTiet>();
}
