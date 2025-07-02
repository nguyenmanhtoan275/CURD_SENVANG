using Xunit;
using Moq;
using OrderService.Application.Services;
using OrderService.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using OrderService.Domain.Interfaces;
using CommonHttpClient.Interfaces;

namespace OrderServiceTest.Application.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<ICustomerApiClient> _customerClientMock;
        private readonly Mock<IProductApiClient> _productClientMock;
        private readonly OrderServiceImpl _service;
        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _customerClientMock = new Mock<ICustomerApiClient>();
            _productClientMock = new Mock<IProductApiClient>();

            _service = new OrderServiceImpl(
                _orderRepoMock.Object,
                _customerClientMock.Object,
                _productClientMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnData_Valid()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(FakeDataOrders());
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(2, result.First().OrderDetails.Count);
            Assert.Equal(100, result.First().Amount); // Check Amount
        }

        [Fact]
        public async Task GetByIdAsync_ReturnData_Valid()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                CustomerId = 10,
                Date = DateTime.Today,
                Amount = 100,
                OrderDetails = new List<OrderDetail>
            {
                new OrderDetail { Id = 1, OrderId = orderId, ProductId = 200, Quantity = 2, UnitPrice = 50 }
            }
            };

            var customer = new CustomerClientDto { Id = 10, Name = "Toan" };
            var productList = new List<ProductClientDto> {
            new ProductClientDto { Id = 200, Name = "Product1" }
                };
            _orderRepoMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _customerClientMock.Setup(x => x.GetCustomerByIdAsync(10)).ReturnsAsync(customer);
            _productClientMock.Setup(x => x.GetProductNamesAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(productList);

            // Act
            var result = await _service.GetByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
            Assert.Equal("Toan", result.CustomerName);
            Assert.Single(result.OrderDetails);
            Assert.Equal("Product1", result.OrderDetails[0].ProductName);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowEx_OrderIsNull()
        {
            _orderRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order) null!);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_ThrowEx_CustomerIsNull()
        {
            _orderRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Order
            {
                Id = 1,
                CustomerId = 999,
                OrderDetails = new List<OrderDetail> { new OrderDetail { ProductId = 200 } }
            });

            _customerClientMock.Setup(x => x.GetCustomerByIdAsync(999)).ReturnsAsync((CustomerClientDto) null!);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_ThrowEx_ProductListEmpty()
        {
            _orderRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Order
            {
                Id = 1,
                CustomerId = 10,
                OrderDetails = new List<OrderDetail> { new OrderDetail { ProductId = 200 } }
            });

            _customerClientMock.Setup(x => x.GetCustomerByIdAsync(10)).ReturnsAsync(new CustomerClientDto { Id = 10, Name = "John" });
            _productClientMock.Setup(x => x.GetProductNamesAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(new List<ProductClientDto>());

            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetFilteredAsync_ReturnCorrectData()
        {
            // Arrange
            var customerList = new List<CustomerClientDto>
            {
                new CustomerClientDto { Id = 10, Name = "Toan" }
            };
            var productList = new List<ProductClientDto> {
            new ProductClientDto { Id = 101, Name = "Product1" }
                };
            _orderRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(FakeDataOrders);
            _customerClientMock.Setup(x => x.GetCustomerNamesAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(customerList);
            _productClientMock.Setup(x => x.GetProductNamesAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(productList);

            // Act
            var result = (await _service.GetFilteredAsync(10, DateTime.Today)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Toan", result[0].CustomerName);
            Assert.Equal(2, result[0].OrderDetails.Count());
            Assert.Equal("Product1", result[0].OrderDetails[0].ProductName);
        }

        private List<Order> FakeDataOrders()
        {
            return new List<Order>
            {
                new Order
                {
                    Id = 1,
                    CustomerId = 10,
                    Date = DateTime.Today,
                    Amount = 100,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 1, OrderId = 1, ProductId = 101, Quantity = 1, UnitPrice = 50 },
                        new OrderDetail { Id = 2, OrderId = 1, ProductId = 102, Quantity = 2, UnitPrice = 25 }
                    }
                }
            };
        }
    }
}