namespace ISPAdmin.Workflow.Domain.StateMachines;

/// <summary>
/// Defines allowed domain lifecycle transitions
/// </summary>
public enum DomainTransition
{
    Register,
    Activate,
    Suspend,
    Renew,
    Expire,
    Cancel,
    TransferIn,
    TransferOut,
    Reactivate
}
