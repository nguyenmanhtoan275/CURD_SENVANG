namespace OrderService.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
