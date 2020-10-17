using System;
using System.Text;
using Convey;
using Convey.Auth;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.Docs.Swagger;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.MessageBrokers;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.Prometheus;
using Convey.Persistence.Redis;
using Convey.Security;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Trill.Services.Timeline.Core;
using Trill.Services.Timeline.Core.Events.External;
using Trill.Services.Timeline.Infrastructure.Decorators;
using Trill.Services.Timeline.Infrastructure.Logging;
using Trill.Services.Timeline.Infrastructure.Redis;

namespace Trill.Services.Timeline.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
            builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));
            
            builder.Services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<IStorage, RedisStorage>();

            builder
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher()
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher()
                .AddJwt()
                .AddHttpClient()
                .AddConsul()
                .AddFabio()
                .AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
                .AddRedis()
                .AddPrometheus()
                .AddJaeger()
                .AddWebApiSwaggerDocs()
                .AddSecurity();

            builder.Services.AddScoped<LogContextMiddleware>()
                .AddSingleton<ICorrelationIdFactory, CorrelationIdFactory>();
            
            return builder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<LogContextMiddleware>()
                .UseSwaggerDocs()
                .UseJaeger()
                .UseConvey()
                .UseAccessTokenValidator()
                .UsePrometheus()
                .UseAuthentication()
                .UseRabbitMq()
                .SubscribeEvent<StorySent>()
                .SubscribeEvent<UserFollowed>()
                .SubscribeEvent<UserUnfollowed>()
                .SubscribeEvent<StoryRated>();

            return app;
        }
        
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
            => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        
        internal static string GetSpanContext(this IMessageProperties messageProperties, string header)
        {
            if (messageProperties is null)
            {
                return string.Empty;
            }

            if (messageProperties.Headers.TryGetValue(header, out var span) && span is byte[] spanBytes)
            {
                return Encoding.UTF8.GetString(spanBytes);
            }

            return string.Empty;
        }
    }
}