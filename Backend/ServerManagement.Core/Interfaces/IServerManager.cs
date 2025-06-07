using ServerManagement.Core.Models;

namespace ServerManagement.Core.Interfaces;

public interface IServerManager
{
    Task<VmStatus> GetVmStatusAsync();
    Task StartVmAsync();
    Task StopVmAsync();
    Task RestartVmAsync();
    Task<GameServerInfo> GetGameInfoAsync();
    Task<IReadOnlyList<PlayerInfo>> GetPlayersAsync();
    // Implementations may throw VmOperationException or GameServerUnreachableException
}