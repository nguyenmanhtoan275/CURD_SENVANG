namespace CommonHttpClient.Interfaces
{
    public interface IProductApiClient
    {
        Task<List<ProductClientDto>> GetProductNamesAsync(IEnumerable<int> ids);
        Task<ProductClientDto> GetProductByIdAsync(int id);
    }
}
