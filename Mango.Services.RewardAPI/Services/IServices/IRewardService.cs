using Mango.Services.RewardAPI.Models.DTOs;

namespace Mango.Services.RewardAPI.Services.IServices
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsDTO rewardsDTO);
    }
}
