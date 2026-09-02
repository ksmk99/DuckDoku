using System.Net;
using DuckDoku.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DuckDoku.Api;

public class ApiExceptionHandler : IExceptionHandler
{
    private static readonly Dictionary<ErrorCode, HttpStatusCode> Statuses =
        new()
        {
            [ErrorCode.InvalidRequest] = HttpStatusCode.BadRequest,
            [ErrorCode.UnsupportedClientVersion] = HttpStatusCode.UpgradeRequired,

            [ErrorCode.TokenExpired] = HttpStatusCode.Unauthorized,
            [ErrorCode.TokenRevoked] = HttpStatusCode.Unauthorized,
            [ErrorCode.DeviceBanned] = HttpStatusCode.Forbidden,

            [ErrorCode.NotEnoughEnergy] = HttpStatusCode.Conflict,
            [ErrorCode.NotEnoughCurrency] = HttpStatusCode.Conflict,
            [ErrorCode.EnergyFull] = HttpStatusCode.Conflict,

            [ErrorCode.LevelLocked] = HttpStatusCode.Forbidden,
            [ErrorCode.SessionExpired] = HttpStatusCode.Conflict,
            [ErrorCode.SessionAlreadyClaimed] = HttpStatusCode.Conflict,
            [ErrorCode.InvalidSolution] = HttpStatusCode.UnprocessableEntity,
            [ErrorCode.VersionConflict] = HttpStatusCode.Conflict,

            [ErrorCode.RateLimited] = HttpStatusCode.TooManyRequests,
            [ErrorCode.ServerUnavailable] = HttpStatusCode.ServiceUnavailable
        };

    private readonly IProblemDetailsService _problemDetailsService;

    public ApiExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ApiException apiException)
        {
            return false;
        }

        int status = (int)StatusFor(apiException.Code);

        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = status,
                Title = apiException.Message,
                Extensions = { ["code"] = apiException.Code.ToString() }
            }
        });
    }

    public static HttpStatusCode StatusFor(ErrorCode code)
    {
        return Statuses.TryGetValue(code, out var status) ? status : HttpStatusCode.BadRequest;
    }
}