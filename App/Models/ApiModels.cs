namespace SevenDTDWebApp.Models;

public enum VmState
{
    Deallocated,
    Deallocating,
    Starting,
    Running,
    Stopping,
    Stopped
}

public class VmStatus
{
    public VmState VmState { get; set; }
    public bool? GamePortOpen { get; set; }
}

public class GameServerInfo
{
    public string Version { get; set; } = "";
    public DateTime ServerTimeUtc { get; set; }
    public int InGameSeconds { get; set; }
    public int InGameDay { get; set; }
    public int TimeScale { get; set; }
    public int DayStartHour { get; set; }
    public int NightStartHour { get; set; }
}

public class PlayerInfo
{
    public string Name { get; set; } = "";
    public bool IsOnline { get; set; }
}

public class ApiErrorResponse
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Details { get; set; }
}