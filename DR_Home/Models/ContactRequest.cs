namespace DR_Home.Models;

public record ContactRequest(string Subject, string Email, string Message, string? VerificationCode = null);
