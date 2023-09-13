namespace Mango.Services.RewardAPI.Models.DTOs
{
    public class RewardsDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime RewardsDate { get; set; }
        public int RewardPoints { get; set; }
        public int OrderId { get; set; }
    }
}
