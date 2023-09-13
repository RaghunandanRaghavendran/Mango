namespace Mango.Web.Models
{
    public class StripeRequestDTO
    {
        public string? StripeSessionURL { get; set; }
        public string? StripeSessionId { get; set; }
        public string? ApprovedURL { get; set; }
        public string? CancelURL { get; set; }
        public OrderDTO Order { get; set; }
    }
}
