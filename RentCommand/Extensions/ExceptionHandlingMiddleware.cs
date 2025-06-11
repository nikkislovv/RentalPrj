using CatalogApi.Logger;
using Domain.Exceptions;
using Domain.Infrastructure;
using SendGrid.Helpers.Errors.Model;
using System.Text.Json;

namespace RentCommandApi.Extensions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILoggerManager logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, DateTime.UtcNow);
                await HandleExceptionAsync(context, e);
            }
        }
        private async Task HandleExceptionAsync(
            HttpContext httpContext,
            Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var response = new
            {
                status = statusCode,
                message = statusCode == 503 
                    ? "Service is unavailable" 
                    : exception.Message,
            };
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        private int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                AggregateNotFoundException => StatusCodes.Status400BadRequest,
                TimeoutException => StatusCodes.Status503ServiceUnavailable,
                ConcurrencyException => StatusCodes.Status400BadRequest,
                Domain.Exceptions.NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
