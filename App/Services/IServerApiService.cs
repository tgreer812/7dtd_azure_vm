using SevenDTDWebApp.Models;

namespace SevenDTDWebApp.Services;

public interface IServerApiService
{
    // VM Management endpoints
    Task<VmStatus> GetVmStatusAsync();
    Task<VmStatus> StartVmAsync();
    Task<VmStatus> StopVmAsync();
    Task<VmStatus> RestartVmAsync();
    
    // Game Server endpoints
    Task<GameServerInfo> GetGameInfoAsync();
    Task<IReadOnlyList<PlayerInfo>> GetPlayersAsync();
}