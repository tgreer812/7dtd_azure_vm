using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;

namespace ServerManagement.Functions.Functions;

public class VmFunctions
{
    private readonly ILogger<VmFunctions> _logger;
    private readonly IServerManager _serverManager;

    public VmFunctions(ILogger<VmFunctions> logger, IServerManager serverManager)
    {
        _logger = logger;
        _serverManager = serverManager;
    }

    [Function("GetVmStatus")]
    public async Task<HttpResponseData> GetVmStatus([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vm/status")] HttpRequestData req)
    {
        try
        {
            var status = await _serverManager.GetVmStatusAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(status);
            return response;
        }
        catch (VmOperationException ex)
        {
            _logger.LogError(ex, "VM operation failed");
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "VM_OPERATION_FAILED", ex.Message, ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting VM status");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred", ex.Message);
        }
    }

    [Function("StartVm")]
    public async Task<HttpResponseData> StartVm([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vm/start")] HttpRequestData req)
    {
        try
        {
            await _serverManager.StartVmAsync();
            var status = await _serverManager.GetVmStatusAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(status);
            return response;
        }
        catch (VmOperationException ex)
        {
            _logger.LogError(ex, "Failed to start VM");
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "VM_START_FAILED", ex.Message, ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error starting VM");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred", ex.Message);
        }
    }

    [Function("StopVm")]
    public async Task<HttpResponseData> StopVm([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vm/stop")] HttpRequestData req)
    {
        try
        {
            await _serverManager.StopVmAsync();
            var status = await _serverManager.GetVmStatusAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(status);
            return response;
        }
        catch (VmOperationException ex)
        {
            _logger.LogError(ex, "Failed to stop VM");
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "VM_STOP_FAILED", ex.Message, ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error stopping VM");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred", ex.Message);
        }
    }

    [Function("RestartVm")]
    public async Task<HttpResponseData> RestartVm([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vm/restart")] HttpRequestData req)
    {
        try
        {
            await _serverManager.RestartVmAsync();
            var status = await _serverManager.GetVmStatusAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(status);
            return response;
        }
        catch (VmOperationException ex)
        {
            _logger.LogError(ex, "Failed to restart VM");
            return await CreateErrorResponse(req, HttpStatusCode.ServiceUnavailable, "VM_RESTART_FAILED", ex.Message, ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error restarting VM");
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