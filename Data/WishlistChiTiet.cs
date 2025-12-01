using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class WishlistChiTiet
{
    public string MaWishlist { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public string MaBienThe { get; set; } = null!;

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;

    public virtual Wishlist MaWishlistNavigation { get; set; } = null!;
}
