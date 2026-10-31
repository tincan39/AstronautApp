using MediatR;
using StargateAPI.Business.Data;
using System.Diagnostics;

namespace StargateAPI.PipelineBehaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly StargateContext _context;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, StargateContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                await WriteLogAsync(requestName, LogEntry.StatusSuccess, stopwatch.ElapsedMilliseconds, null);
                return response;
            }
            catch (Exception e)
            {
                _context.ChangeTracker.Clear();
                await WriteLogAsync(requestName, LogEntry.StatusError, stopwatch.ElapsedMilliseconds, e);
                throw;
            }
        }

        private async Task WriteLogAsync(string requestName, string status, long elapsedMs, Exception? exception)
        {
            try
            {
                _context.Logs.Add(new LogEntry
                {
                    Timestamp = DateTime.UtcNow,
                    RequestName = requestName,
                    Status = status,
                    ElapsedMs = elapsedMs,
                    ExceptionMessage = exception?.Message,
                    StackTrace = exception?.ToString()
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception logException)
            {
                _logger.LogError(logException, "Failed to write log entry for {RequestName}", requestName);
            }
        }
    }
}
