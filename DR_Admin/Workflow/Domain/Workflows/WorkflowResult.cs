namespace ISPAdmin.Workflow.Domain.Workflows;

/// <summary>
/// Result of a workflow execution
/// </summary>
public class WorkflowResult
{
    public bool IsSuccess { get; set; }
    public int? AggregateId { get; set; }
    public string? CorrelationId { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }

    public static WorkflowResult Success(int aggregateId, string? message = null, string? correlationId = null)
    {
        return new WorkflowResult
        {
            IsSuccess = true,
            AggregateId = aggregateId,
            Message = message,
            CorrelationId = correlationId
        };
    }

    public static WorkflowResult Failed(string errorMessage, string? correlationId = null)
    {
        return new WorkflowResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            CorrelationId = correlationId
        };
    }
}
