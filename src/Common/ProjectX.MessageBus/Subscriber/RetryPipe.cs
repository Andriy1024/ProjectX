using Microsoft.Extensions.Logging;
using Polly;
using ProjectX.Core.Exceptions;
using System;
using System.Threading.Tasks;
using static ProjectX.RabbitMq.Pipeline.Pipe;

namespace ProjectX.RabbitMq.Pipeline
{
    /// <summary>
    /// Represents retry policy for RabbitMq consumer.
    /// </summary>
    internal sealed class RetryPipe : IPipeLine<SubscriberRequest>
    {
        /// <summary>
        /// Retry tiggered if thw handler retruns false;
        /// </summary>
        public delegate bool ErrorHandler(Exception ex);

        private readonly ILogger _logger;

        private ErrorHandler _handleError;

        public RetryPipe(ILogger logger)
            : this(logger, DefaultErrorHandler)
        {
        }

        public RetryPipe(ILogger logger, ErrorHandler errorHandler)
        {
            _logger = logger;
            _handleError = errorHandler;
        }

        public async Task Handle(SubscriberRequest message, Handler<SubscriberRequest> next)
        {
            await Policy
                  .Handle<Exception>()
                  .WaitAndRetryAsync(2, i => TimeSpan.FromSeconds(2))
                  .ExecuteAsync(async () =>
                  {
                      try
                      {
                          await next(message);
                      }
                      catch (Exception ex)
                      {
                          _logger.LogError(ex, ex.Message);

                          if (_handleError(ex))
                              return;

                          throw;
                      }
                  });
        }

        private static bool DefaultErrorHandler(Exception ex) => ex switch
        {
            InvalidPermissionException e => true,
            InvalidDataException e => true,
            NotFoundException e => true,
            _ => false
        };
    }
}
