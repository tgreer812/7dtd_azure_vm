namespace ServerManagement.Core.Models;

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