namespace EvacuationPlanningandMonitoringAPI.Helpers
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new { message = "Internal Server Error", error = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
