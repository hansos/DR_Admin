using ISPAdmin.Data.Enums;

namespace ISPAdmin.Workflow.Domain.StateMachines;

/// <summary>
/// Defines allowed invoice lifecycle transitions
/// </summary>
public enum InvoiceTransition
{
    Send,
    Pay,
    PartialPay,
    Void,
    MarkOverdue,
    Refund
}

/// <summary>
/// State machine for invoice lifecycle transitions
/// </summary>
public class InvoiceStateMachine
{
    private static readonly Dictionary<(InvoiceStatus From, InvoiceTransition Transition), InvoiceStatus> _transitions = new()
    {
        // Draft invoice transitions
        { (InvoiceStatus.Draft, InvoiceTransition.Send), InvoiceStatus.Issued },
        { (InvoiceStatus.Draft, InvoiceTransition.Void), InvoiceStatus.Cancelled },
        
        // Issued invoice transitions
        { (InvoiceStatus.Issued, InvoiceTransition.Pay), InvoiceStatus.Paid },
        { (InvoiceStatus.Issued, InvoiceTransition.MarkOverdue), InvoiceStatus.Overdue },
        { (InvoiceStatus.Issued, InvoiceTransition.Void), InvoiceStatus.Cancelled },
        
        // Overdue invoice transitions
        { (InvoiceStatus.Overdue, InvoiceTransition.Pay), InvoiceStatus.Paid },
        { (InvoiceStatus.Overdue, InvoiceTransition.Void), InvoiceStatus.Cancelled },
        
        // Paid invoice transitions
        { (InvoiceStatus.Paid, InvoiceTransition.Refund), InvoiceStatus.Credited }
    };

    public static bool CanTransition(InvoiceStatus currentStatus, InvoiceTransition transition)
    {
        return _transitions.ContainsKey((currentStatus, transition));
    }

    public static InvoiceStatus Transition(InvoiceStatus currentStatus, InvoiceTransition transition)
    {
        if (!_transitions.TryGetValue((currentStatus, transition), out var newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid invoice transition from {currentStatus} via {transition}");
        }
        return newStatus;
    }

    public static IEnumerable<InvoiceTransition> GetValidTransitions(InvoiceStatus currentStatus)
    {
        return _transitions
            .Where(kvp => kvp.Key.From == currentStatus)
            .Select(kvp => kvp.Key.Transition)
            .Distinct();
    }
}
