using Microsoft.AspNetCore.Mvc;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;
using ServerManagement.Core.Exceptions;

namespace ServerManagment.WebApi;

public static class VmEndpoints
{
    public static void MapVmEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/vm/status", async ([FromServices] IServerManager manager, ILogger<VmEndpoints> logger) =>
        {
            try
            {
                var status = await manager.GetVmStatusAsync();
                return Results.Ok(status);
            }
            catch (VmOperationException ex)
            {
                logger.LogError(ex, "VM operation failed");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "VM_OPERATION_FAILED",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message ?? string.Empty
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting VM status");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                }, statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        app.MapPost("/api/vm/start", async ([FromServices] IServerManager manager, ILogger<VmEndpoints> logger) =>
        {
            try
            {
                await manager.StartVmAsync();
                var status = await manager.GetVmStatusAsync();
                return Results.Ok(status);
            }
            catch (VmOperationException ex)
            {
                logger.LogError(ex, "Failed to start VM");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "VM_START_FAILED",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message ?? string.Empty
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error starting VM");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                }, statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        app.MapPost("/api/vm/stop", async ([FromServices] IServerManager manager, ILogger<VmEndpoints> logger) =>
        {
            try
            {
                await manager.StopVmAsync();
                var status = await manager.GetVmStatusAsync();
                return Results.Ok(status);
            }
            catch (VmOperationException ex)
            {
                logger.LogError(ex, "Failed to stop VM");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "VM_STOP_FAILED",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message ?? string.Empty
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error stopping VM");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                }, statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        app.MapPost("/api/vm/restart", async ([FromServices] IServerManager manager, ILogger<VmEndpoints> logger) =>
        {
            try
            {
                await manager.RestartVmAsync();
                var status = await manager.GetVmStatusAsync();
                return Results.Ok(status);
            }
            catch (VmOperationException ex)
            {
                logger.LogError(ex, "Failed to restart VM");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "VM_RESTART_FAILED",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message ?? string.Empty
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error restarting VM");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                }, statusCode: StatusCodes.Status500InternalServerError);
            }
        });
    }
}
