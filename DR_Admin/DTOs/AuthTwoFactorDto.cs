namespace ISPAdmin.DTOs;

/// <summary>
/// Request DTO for verifying a two-factor authentication code.
/// </summary>
public class VerifyMailTwoFactorRequestDto
{
    /// <summary>
    /// Gets or sets the temporary challenge token returned by login.
    /// </summary>
    public string ChallengeToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the one-time code from the selected second-factor method.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for resending a mail two-factor authentication code.
/// </summary>
public class ResendMailTwoFactorRequestDto
{
    /// <summary>
    /// Gets or sets the temporary challenge token returned by login.
    /// </summary>
    public string ChallengeToken { get; set; } = string.Empty;
}
