using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedException ex)
        {
            await EscribirError(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await EscribirError(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await EscribirError(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await EscribirError(context, StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    private static Task EscribirError(HttpContext context, int statusCode, string mensaje)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { mensaje });
    }
}
