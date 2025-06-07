namespace ServerManagement.Core.Models;

public class VmStatus
{
    // Maps to Azure VM PowerState (via instanceView.statuses)
    public VmState VmState { get; set; }
    public bool? GamePortOpen { get; set; }
}