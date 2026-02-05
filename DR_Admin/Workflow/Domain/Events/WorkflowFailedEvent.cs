namespace ISPAdmin.Workflow.Domain.Events;

/// <summary>
/// Event raised when a workflow fails
/// </summary>
public class WorkflowFailedEvent : DomainEventBase
{
    public override string EventType => "WorkflowFailed";
    
    public string WorkflowType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
}
