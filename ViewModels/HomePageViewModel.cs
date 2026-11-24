using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class HomePageViewModel
{
    public List<HomeCatalogShelf> Shelves { get; set; } = [];
}

public sealed class HomeCatalogShelf
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<CatalogItem> Items { get; set; } = [];
}
