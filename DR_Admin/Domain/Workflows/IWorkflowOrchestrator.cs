namespace ISPAdmin.Domain.Workflows;

/// <summary>
/// Base interface for workflow orchestrators
/// </summary>
public interface IWorkflowOrchestrator
{
}

/// <summary>
/// Domain registration workflow orchestrator
/// </summary>
public interface IDomainRegistrationWorkflow : IWorkflowOrchestrator
{
    /// <summary>
    /// Executes the domain registration workflow
    /// </summary>
    Task<WorkflowResult> ExecuteAsync(DomainRegistrationWorkflowInput input);
    
    /// <summary>
    /// Continues the workflow after payment is received
    /// </summary>
    Task<WorkflowResult> OnPaymentReceivedAsync(int orderId, int invoiceId);
}

/// <summary>
/// Domain renewal workflow orchestrator
/// </summary>
public interface IDomainRenewalWorkflow : IWorkflowOrchestrator
{
    /// <summary>
    /// Executes the domain renewal workflow
    /// </summary>
    Task<WorkflowResult> ExecuteAsync(int domainId);
    
    /// <summary>
    /// Processes auto-renewal for a domain
    /// </summary>
    Task<WorkflowResult> ProcessAutoRenewalAsync(int domainId);
}

/// <summary>
/// Order provisioning workflow orchestrator
/// </summary>
public interface IOrderProvisioningWorkflow : IWorkflowOrchestrator
{
    /// <summary>
    /// Provisions a service for an activated order
    /// </summary>
    Task<WorkflowResult> ProvisionAsync(int orderId);
}
