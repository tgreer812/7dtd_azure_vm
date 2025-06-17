namespace ServerManagement.Azure.Configuration;

/// <summary>
/// Configuration for Azure VM operations.
/// Populated from environment variables with prefix "AzureVmConfiguration__"
/// </summary>
public class AzureVmConfiguration
{
    /// <summary>
    /// Azure Subscription ID where the VM is located
    /// Environment variable: AzureVmConfiguration__SubscriptionId
    /// </summary>
    public string SubscriptionId { get; set; } = "";
    
    /// <summary>
    /// Azure Resource Group containing the VM
    /// Environment variable: AzureVmConfiguration__ResourceGroupName
    /// </summary>
    public string ResourceGroupName { get; set; } = "";
    
    /// <summary>
    /// Name of the Azure VM to manage
    /// Environment variable: AzureVmConfiguration__VmName
    /// </summary>
    public string VmName { get; set; } = "";
}

/// <summary>
/// Configuration for connecting to the game server.
/// Populated from environment variables with prefix "GameServerConfiguration__"
/// </summary>
public class GameServerConfiguration
{
    /// <summary>
    /// Game server hostname or IP address
    /// Environment variable: GameServerConfiguration__Host
    /// </summary>
    public string Host { get; set; } = "";
    
    /// <summary>
    /// Game server port (default: 26900)
    /// Environment variable: GameServerConfiguration__Port
    /// </summary>
    public int Port { get; set; } = 26900;
    
    /// <summary>
    /// Telnet port for server administration (default: 8081)
    /// Environment variable: GameServerConfiguration__TelnetPort
    /// </summary>
    public int TelnetPort { get; set; } = 8081;
    
    /// <summary>
    /// Admin password for telnet authentication
    /// Environment variable: GameServerConfiguration__AdminPassword
    /// </summary>
    public string AdminPassword { get; set; } = "";
}