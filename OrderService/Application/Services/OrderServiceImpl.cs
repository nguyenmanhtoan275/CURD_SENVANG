using CommonHttpClient.Interfaces;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using System.Net.Http;

namespace OrderService.Application.Services
{
    public class OrderServiceImpl
    {
        private readonly IOrderRepository _repository;
        private readonly ICustomerApiClient _customerClient;
        private readonly IProductApiClient _productApiClient;

        public OrderServiceImpl(IOrderRepository repository, ICustomerApiClient customerClient, IProductApiClient productApiClient)
        {
            _repository = repository;
            _customerClient = customerClient;
            _productApiClient = productApiClient;
        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null || !order.OrderDetails.Any())
            {
                throw new ArgumentException("Invalid Order.");
            } 

            var customerResponse = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
            if (customerResponse == null)
                throw new ArgumentException("Invalid Customer ID.");

            var products = await _productApiClient.GetProductNamesAsync(order.OrderDetails.Select(x => x.Id));
            if (!products.Any())
            {
                throw new ArgumentException("Invalid Order.");
            }
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = customerResponse.Name,
                Date = order.Date,
                Amount = order.Amount,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = products.FirstOrDefault(x => x.Id == od.ProductId)?.Name ?? "",
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Date = o.Date,
                Amount = o.Amount,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            });
        }

        public async Task<IEnumerable<OrderDto>> GetFilteredAsync(int? customerId, DateTime? date)
        {
            var orders = await _repository.GetAllAsync();

            if (customerId.HasValue)
                orders = orders.Where(o => o.CustomerId == customerId.Value);

            if (date.HasValue)
                orders = orders.Where(o => o.Date.Date == date.Value.Date);

            if (!orders.Any())
                return new List<OrderDto>();
            ;

            var customerResponse = await _customerClient.GetCustomerNamesAsync(orders.Select(x => x.CustomerId));

            var productIds = orders.SelectMany(o => o.OrderDetails)
                                    .Select(od => od.ProductId)
                                    .Distinct()
                                    .ToList();
            var products = await _productApiClient.GetProductNamesAsync(productIds);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = customerResponse.Any() ? customerResponse.FirstOrDefault(x=>x.Id == o.CustomerId)?.Name : string.Empty,
                Date = o.Date,
                Amount = o.Amount,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = products.Any() ? products.FirstOrDefault(x => x.Id == od.ProductId)?.Name : string.Empty,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            });
        }

        public async Task AddAsync(OrderDto orderDto)
        {
            #region Validation
            if (orderDto.OrderDetails.Any(od => od.Quantity <= 0))
                throw new ArgumentException("Order detail quantity must be positive.");
            if (orderDto.Amount < 0)
                throw new ArgumentException("Order amount cannot be negative.");

            // Validate CustomerId
            var customerResponse = await _customerClient.GetCustomerByIdAsync(orderDto.CustomerId);
            if (customerResponse == null)
                throw new ArgumentException("Invalid Customer ID.");

            // Validate ProductIds in OrderDetails
            foreach (var detail in orderDto.OrderDetails)
            {
                var productResponse = await _productApiClient.GetProductByIdAsync(detail.ProductId);
                if (productResponse == null)
                    throw new ArgumentException($"Invalid Product ID: {detail.ProductId}.");
            }
            #endregion

            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                Date = orderDto.Date,
                Amount = orderDto.Amount,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };
            await _repository.AddAsync(order);
        }

        public async Task UpdateAsync(OrderDto orderDto)
        {
            #region Validation
            if (orderDto.OrderDetails.Any(od => od.Quantity <= 0))
                throw new ArgumentException("Order detail quantity must be positive.");
            if (orderDto.Amount < 0)
                throw new ArgumentException("Order amount cannot be negative.");

            // Validate CustomerId
            var customerResponse = await _customerClient.GetCustomerByIdAsync(orderDto.CustomerId);
            if (customerResponse == null)
                throw new ArgumentException("Invalid Customer ID.");

            // Validate ProductIds in OrderDetails
            foreach (var detail in orderDto.OrderDetails)
            {
                var productResponse = await _productApiClient.GetProductByIdAsync(detail.ProductId);
                if (productResponse == null)
                    throw new ArgumentException($"Invalid Product ID: {detail.ProductId}.");
            }
            #endregion

            var order = new Order
            {
                Id = orderDto.Id,
                CustomerId = orderDto.CustomerId,
                Date = orderDto.Date,
                Amount = orderDto.Amount,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };
            await _repository.UpdateAsync(order);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
