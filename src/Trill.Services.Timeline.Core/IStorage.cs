using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trill.Services.Timeline.Core.Data;

namespace Trill.Services.Timeline.Core
{
    public interface IStorage
    {
        Task<Paged<Story>> GetTimelineAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<IReadOnlyCollection<Guid>> GetAllFollowersAsync(Guid userId);
        Task FollowAsync(Guid followerId, Guid followeeId);
        Task UnfollowAsync(Guid followerId, Guid followeeId);
        Task AddStoryAsync(Story story);
        Task SetStoryTotalRatingAsync(long storyId, int totalRate);
        Task AddStoryToTimelineAsync(Guid userId, Story story);
    }
}