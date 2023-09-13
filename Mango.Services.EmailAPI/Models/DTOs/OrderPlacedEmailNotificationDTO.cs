namespace Mango.Services.EmailAPI.Models.DTOs
{
    public class OrderPlacedEmailNotificationDTO
    {
        public string? UserId { get; set; }
        public DateTime RewardsDate { get; set; }
        public int RewardPoints { get; set; }
        public int OrderId { get; set; }
    }
}
