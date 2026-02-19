namespace DomainRegistrationLib.Infrastructure.Settings
{
    /// <summary>
    /// Global sandbox mode settings. When enabled, sandbox registrars are used instead of real provider implementations.
    /// All registrations, renewals, transfers and changes are simulated with dummy data.
    /// </summary>
    public class SandboxSettings
    {
        /// <summary>
        /// Master switch to enable or disable sandbox mode globally
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Per-module sandbox filters. When a module filter is true and Enabled is true,
        /// that module operates in sandbox mode.
        /// </summary>
        public SandboxFilters Filters { get; set; } = new();
    }

    /// <summary>
    /// Per-module sandbox filters controlling which libraries operate in sandbox mode
    /// </summary>
    public class SandboxFilters
    {
        /// <summary>
        /// When true (and global Enabled is true), all domain registration operations
        /// are handled by the SandboxRegistrar instead of the configured provider.
        /// </summary>
        public bool DomainRegistrationLib { get; set; }
    }
}
