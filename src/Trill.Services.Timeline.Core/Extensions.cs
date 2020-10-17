using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;

namespace Trill.Services.Timeline.Core
{
    public static class Extensions
    {
        public static IConveyBuilder AddCore(this IConveyBuilder builder)
        {
            builder
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher()
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher();

            return builder;
        }
    }
}