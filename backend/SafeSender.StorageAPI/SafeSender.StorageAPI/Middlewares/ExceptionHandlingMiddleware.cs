using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using AnonFilesApi.Exceptions;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using SafeSender.StorageAPI.Extensions;

namespace SafeSender.StorageAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError($"File not found: {ex}");
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
        }
        catch (InvalidDownloadLinkException ex)
        {
            _logger.LogError($"Invalid download link: {ex}");
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unknown exception: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context,
        Exception exception,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message.NullIfEmpty() ?? $"Unknown exception occured",
        }.ToString());
    }
}

internal class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = default!;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}