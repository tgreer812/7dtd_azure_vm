namespace ServerManagement.Azure.Configuration;

public class AzureVmConfiguration
{
    public string SubscriptionId { get; set; } = "";
    public string ResourceGroupName { get; set; } = "";
    public string VmName { get; set; } = "";
}

public class GameServerConfiguration
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 26900;
    public int TelnetPort { get; set; } = 8081;
    public string AdminPassword { get; set; } = "";
}