using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;

namespace ServerManagement.Functions.Functions;

public class GameFunctions
{
    private readonly ILogger<GameFunctions> _logger;
    private readonly IServerManager _serverManager;

    public GameFunctions(ILogger<GameFunctions> logger, IServerManager serverManager)
    {
        _logger = logger;
        _serverManager = serverManager;
    }

    [Function("GetGameInfo")]
    public async Task<HttpResponseData> GetGameInfo([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "game/info")] HttpRequestData req)
    {
        try
        {
            var gameInfo = await _serverManager.GetGameInfoAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(gameInfo);
            return response;
        }
        catch (GameServerUnreachableException ex)
        {
            _logger.LogError(ex, "Game server unreachable");
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "GAME_SERVER_UNREACHABLE", ex.Message, ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting game info");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred", ex.Message);
        }
    }

    [Function("GetPlayers")]
    public async Task<HttpResponseData> GetPlayers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "game/players")] HttpRequestData req)
    {
        try
        {
            var players = await _serverManager.GetPlayersAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(players);
            return response;
        }
        catch (GameServerUnreachableException ex)
        {
            _logger.LogError(ex, "Game server unreachable");
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "GAME_SERVER_UNREACHABLE", ex.Message, ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting players");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred", ex.Message);
        }
    }

    private static async Task<HttpResponseData> CreateErrorResponse(HttpRequestData req, HttpStatusCode statusCode, string code, string message, string? details = null)
    {
        var response = req.CreateResponse(statusCode);
        var errorResponse = new ApiErrorResponse
        {
            Code = code,
            Message = message,
            Details = details ?? ""
        };
        await response.WriteAsJsonAsync(errorResponse);
        return response;
    }
}