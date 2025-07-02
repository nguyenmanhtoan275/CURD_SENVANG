namespace CommonHttpClient.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<List<CustomerClientDto>> GetCustomerNamesAsync(IEnumerable<int> ids);
        Task<CustomerClientDto> GetCustomerByIdAsync(int id);
    }
}
