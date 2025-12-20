using System.Collections.Generic;

namespace KIDORA.ViewModels
{
    public class PageResultVM
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Snippet { get; set; } = string.Empty;
    }

    public class SearchResultVM
    {
        public string Keyword { get; set; } = string.Empty;

        public List<ListSanPhamVM> Products { get; set; } = new List<ListSanPhamVM>();

        public List<CamNangVM> CamNangs { get; set; } = new List<CamNangVM>();

        public List<PageResultVM> Pages { get; set; } = new List<PageResultVM>();
    }
}
