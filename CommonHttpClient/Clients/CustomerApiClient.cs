using System.Net.Http;
using System.Net.Http.Json;
using CommonHttpClient.Interfaces;

namespace CommonHttpClient.Clients
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerClientDto> GetCustomerByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/customer/{id}");
            response.EnsureSuccessStatusCode();
            var customer = await response.Content.ReadFromJsonAsync<CustomerClientDto>();
            return customer!;
        }

        public async Task<List<CustomerClientDto>> GetCustomerNamesAsync(IEnumerable<int> ids)
        {
            var response = await _httpClient.PostAsJsonAsync("api/customer/get-list", ids);
            response.EnsureSuccessStatusCode();
            var customers = await response.Content.ReadFromJsonAsync<List<CustomerClientDto>>();
            return customers!;
        }
    }
}