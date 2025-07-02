using System.Net.Http;
using System.Net.Http.Json;
using CommonHttpClient.Interfaces;

namespace CommonHttpClient.Clients
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;

        public ProductApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductClientDto> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<ProductClientDto>();
            return product!;
        }

        public async Task<List<ProductClientDto>> GetProductNamesAsync(IEnumerable<int> ids)
        {
            var response = await _httpClient.PostAsJsonAsync("api/products/get-list", ids);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadFromJsonAsync<List<ProductClientDto>>();
            return products!;
        }
    }
}