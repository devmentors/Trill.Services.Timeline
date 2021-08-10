using System.Threading.Tasks;
using Convey.CQRS.Events;

namespace Trill.Services.Timeline.Core.Events.External.Handlers
{
    public sealed class StoryRatedHandler : IEventHandler<StoryRated>
    {
        private readonly IStorage _storage;

        public StoryRatedHandler(IStorage storage)
        {
            _storage = storage;
        }
    
        public async Task HandleAsync(StoryRated @event)
        {
            await _storage.SetStoryTotalRatingAsync(@event.StoryId, @event.TotalRate);
        }
    }
}