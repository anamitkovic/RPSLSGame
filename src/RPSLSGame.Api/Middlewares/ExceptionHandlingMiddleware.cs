using System.Net;
using System.Text.Json;
using Npgsql;

namespace RPSLSGame.Api.Middlewares;
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred while processing request");
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var baseException = exception.GetBaseException();

        object errorResponse;

        switch (baseException)
        {
            case ArgumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new { message = exception.Message };
                break;

            case NpgsqlException:
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse = new { message = "Database is currently unavailable." };
                break;
            
            case HttpRequestException:
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse = new { message = "External service is currently unavailable. Please try again later." };
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new { message = "An unexpected error occurred." };
                break;
        }

        return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

}
