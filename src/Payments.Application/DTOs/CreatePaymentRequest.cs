using System.ComponentModel.DataAnnotations;

namespace Payments.Application.DTOs;

public sealed class CreatePaymentRequest
{
    [Required(ErrorMessage = "order_id is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "order_id must be between 1 and 100 characters")]
    public string OrderId { get; set; } = string.Empty;

    [Required(ErrorMessage = "value is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "value must be greater than zero")]
    public decimal Value { get; set; }
}
