using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [MinLength(1, ErrorMessage = "At least one order detail is required")]
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
