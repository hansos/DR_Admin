using ISPAdmin.Data.Enums;

namespace ISPAdmin.Domain.StateMachines;

/// <summary>
/// State machine for domain lifecycle transitions
/// Enforces valid state transitions and business rules
/// </summary>
public class DomainStateMachine
{
    private static readonly Dictionary<(DomainStatus From, DomainTransition Transition), DomainStatus> _transitions = new()
    {
        // Registration flow
        { (DomainStatus.PendingRegistration, DomainTransition.Register), DomainStatus.Active },
        { (DomainStatus.PendingRegistration, DomainTransition.Cancel), DomainStatus.Cancelled },
        
        // Active domain transitions
        { (DomainStatus.Active, DomainTransition.Suspend), DomainStatus.Suspended },
        { (DomainStatus.Active, DomainTransition.Renew), DomainStatus.Active },
        { (DomainStatus.Active, DomainTransition.Expire), DomainStatus.Expired },
        { (DomainStatus.Active, DomainTransition.TransferOut), DomainStatus.TransferredOut },
        { (DomainStatus.Active, DomainTransition.Cancel), DomainStatus.Cancelled },
        
        // Suspended domain transitions
        { (DomainStatus.Suspended, DomainTransition.Reactivate), DomainStatus.Active },
        { (DomainStatus.Suspended, DomainTransition.Cancel), DomainStatus.Cancelled },
        { (DomainStatus.Suspended, DomainTransition.Expire), DomainStatus.Expired },
        
        // Expired domain transitions
        { (DomainStatus.Expired, DomainTransition.Renew), DomainStatus.Active },
        { (DomainStatus.Expired, DomainTransition.Cancel), DomainStatus.Cancelled },
        
        // Transfer flow
        { (DomainStatus.PendingTransfer, DomainTransition.TransferIn), DomainStatus.Active },
        { (DomainStatus.PendingTransfer, DomainTransition.Cancel), DomainStatus.Cancelled },
        
        // Pending renewal
        { (DomainStatus.PendingRenewal, DomainTransition.Renew), DomainStatus.Active },
        { (DomainStatus.PendingRenewal, DomainTransition.Expire), DomainStatus.Expired }
    };

    /// <summary>
    /// Checks if a transition is allowed from the current status
    /// </summary>
    public static bool CanTransition(DomainStatus currentStatus, DomainTransition transition)
    {
        return _transitions.ContainsKey((currentStatus, transition));
    }

    /// <summary>
    /// Performs a state transition, throwing if invalid
    /// </summary>
    public static DomainStatus Transition(DomainStatus currentStatus, DomainTransition transition)
    {
        if (!_transitions.TryGetValue((currentStatus, transition), out var newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid domain transition from {currentStatus} via {transition}");
        }
        return newStatus;
    }

    /// <summary>
    /// Gets all valid transitions from a given status
    /// </summary>
    public static IEnumerable<DomainTransition> GetValidTransitions(DomainStatus currentStatus)
    {
        return _transitions
            .Where(kvp => kvp.Key.From == currentStatus)
            .Select(kvp => kvp.Key.Transition)
            .Distinct();
    }
}
