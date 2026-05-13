namespace Customer.Services;

using Shared.ViewModels;

public interface IProductApiService
{
    Task<List<ProductCardViewModel>> GetProductCatalogAsync();
    Task<ProductDetailsViewModel?> GetProductByIdAsync(int id);
}