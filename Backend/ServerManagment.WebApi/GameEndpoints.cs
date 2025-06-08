using Microsoft.AspNetCore.Mvc;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;

namespace ServerManagment.WebApi;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/game/info", async ([FromServices] IServerManager manager, ILogger<GameEndpoints> logger) =>
        {
            try
            {
                var info = await manager.GetGameInfoAsync();
                return Results.Ok(info);
            }
            catch (GameServerUnreachableException ex)
            {
                logger.LogError(ex, "Game server unreachable");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "GAME_SERVER_UNREACHABLE",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message ?? string.Empty
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting game info");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                }, statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        app.MapGet("/api/game/players", async ([FromServices] IServerManager manager, ILogger<GameEndpoints> logger) =>
        {
            try
            {
                var players = await manager.GetPlayersAsync();
                return Results.Ok(players);
            }
            catch (GameServerUnreachableException ex)
            {
                logger.LogError(ex, "Game server unreachable");
                return Results.Json(new ApiErrorResponse
                {
                    Code = "GAME_SERVER_UNREACHABLE",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message ?? string.Empty
                }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting players");
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
