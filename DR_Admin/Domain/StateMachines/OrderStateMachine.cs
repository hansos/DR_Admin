using ISPAdmin.Data.Enums;

namespace ISPAdmin.Domain.StateMachines;

/// <summary>
/// Defines allowed order lifecycle transitions
/// </summary>
public enum OrderTransition
{
    Activate,
    Suspend,
    Resume,
    Cancel,
    Expire,
    Renew
}

/// <summary>
/// State machine for order lifecycle transitions
/// </summary>
public class OrderStateMachine
{
    private static readonly Dictionary<(OrderStatus From, OrderTransition Transition), OrderStatus> _transitions = new()
    {
        // Pending order transitions
        { (OrderStatus.Pending, OrderTransition.Activate), OrderStatus.Active },
        { (OrderStatus.Pending, OrderTransition.Cancel), OrderStatus.Cancelled },
        
        // Active order transitions
        { (OrderStatus.Active, OrderTransition.Suspend), OrderStatus.Suspended },
        { (OrderStatus.Active, OrderTransition.Expire), OrderStatus.Expired },
        { (OrderStatus.Active, OrderTransition.Cancel), OrderStatus.Cancelled },
        { (OrderStatus.Active, OrderTransition.Renew), OrderStatus.Active },
        
        // Suspended order transitions
        { (OrderStatus.Suspended, OrderTransition.Resume), OrderStatus.Active },
        { (OrderStatus.Suspended, OrderTransition.Cancel), OrderStatus.Cancelled },
        { (OrderStatus.Suspended, OrderTransition.Expire), OrderStatus.Expired },
        
        // Expired order transitions
        { (OrderStatus.Expired, OrderTransition.Renew), OrderStatus.Active }
    };

    public static bool CanTransition(OrderStatus currentStatus, OrderTransition transition)
    {
        return _transitions.ContainsKey((currentStatus, transition));
    }

    public static OrderStatus Transition(OrderStatus currentStatus, OrderTransition transition)
    {
        if (!_transitions.TryGetValue((currentStatus, transition), out var newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid order transition from {currentStatus} via {transition}");
        }
        return newStatus;
    }

    public static IEnumerable<OrderTransition> GetValidTransitions(OrderStatus currentStatus)
    {
        return _transitions
            .Where(kvp => kvp.Key.From == currentStatus)
            .Select(kvp => kvp.Key.Transition)
            .Distinct();
    }
}
