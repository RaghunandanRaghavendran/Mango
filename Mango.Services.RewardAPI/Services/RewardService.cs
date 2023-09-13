using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Models;
using Mango.Services.RewardAPI.Models.DTOs;
using Mango.Services.RewardAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;

        public RewardService(DbContextOptions<ApplicationDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }
        public async Task UpdateRewards(RewardsDTO rewardsDTO)
        {
            try
            {
                Reward rewards = new()
                {
                    OrderId = rewardsDTO.OrderId,
                    RewardPoints = rewardsDTO.RewardPoints,
                    UserId = rewardsDTO.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var _db = new ApplicationDbContext(_dbOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
