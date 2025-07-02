
using CommonHttpClient.Interfaces;
using CommonHttpClient.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public static class HttpClientRegistration
{
    public static IServiceCollection AddExternalHttpClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
        {
            client.BaseAddress = new Uri(config["Services:CustomerService"]);
        });
        services.AddHttpClient<IProductApiClient, ProductApiClient>(client =>
        {
            client.BaseAddress = new Uri(config["Services:ProductService"]);
        });

        return services;
    }
}