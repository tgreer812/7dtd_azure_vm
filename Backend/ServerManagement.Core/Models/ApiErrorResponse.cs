namespace ServerManagement.Core.Models;

public class ApiErrorResponse
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
    public string Details { get; set; } = "";
}