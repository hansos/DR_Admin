using DR_Home.Models;
using DR_Home.Services;

namespace DR_Home
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSingleton<ContactVerificationService>();

            // Bind email settings from configuration
            var emailSettings = builder.Configuration.GetSection("EmailSettings:Smtp").Get<EmailSmtpSettings>();
            if (emailSettings != null)
            {
                builder.Services.AddSingleton(emailSettings);
                builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            }

            var app = builder.Build();

            // Enable serving static files from wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Minimal API endpoint for health check (future extensibility)
            app.MapGet("/api/health", () => Results.Ok(new { status = "Not running", timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck");

            // Minimal API endpoint to receive contact form submissions
            app.MapPost("/api/contact", async (
                ContactRequest request,
                IEmailSender emailSender,
                ContactVerificationService verification) =>
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Message))
                    return Results.BadRequest(new { status = "error", message = "Email and message are required." });

                var email = request.Email.Trim().ToLowerInvariant();

                // Check blacklist
                if (verification.IsBlacklisted(email))
                    return Results.Json(
                        new { status = "blocked", message = "This email address has been blocked due to too many failed attempts." },
                        statusCode: 403);

                var subject = string.IsNullOrWhiteSpace(request.Subject)
                    ? "DR_Admin Commercial License Inquiry"
                    : request.Subject;

                var body =
                    "DR_Admin contact request" + Environment.NewLine +
                    "--------------------------------" + Environment.NewLine +
                    "From: " + email + Environment.NewLine +
                    "Subject: " + subject + Environment.NewLine +
                    Environment.NewLine +
                    request.Message;

                // Already verified — send directly (with rate limit)
                if (verification.IsVerified(email))
                {
                    if (!verification.CanSendMessage(email))
                    {
                        var remaining = verification.GetRateLimitRemainingSeconds(email) ?? 300;
                        return Results.Json(
                            new { status = "rate_limited", message = "You have reached the message limit. Please try again shortly.", retryAfterSeconds = remaining },
                            statusCode: 429);
                    }

                    try
                    {
                        await emailSender.SendAsync(subject, body, email);
                        verification.RecordMessageSent(email);
                        return Results.Ok(new { status = "ok" });
                    }
                    catch (Exception ex)
                    {
                        return Results.Json(
                            new { status = "error", message = "Failed to send message.", detail = ex.Message },
                            statusCode: 500);
                    }
                }

                // Verification code was provided — validate it
                if (!string.IsNullOrWhiteSpace(request.VerificationCode))
                {
                    var result = verification.ValidateCode(email, request.VerificationCode);

                    switch (result)
                    {
                        case VerificationResult.Success:
                            try
                            {
                                await emailSender.SendAsync(subject, body, email);
                                verification.RecordMessageSent(email);
                                return Results.Ok(new { status = "ok" });
                            }
                            catch (Exception ex)
                            {
                                return Results.Json(
                                    new { status = "error", message = "Failed to send message.", detail = ex.Message },
                                    statusCode: 500);
                            }

                        case VerificationResult.WrongCode:
                            var attemptsLeft = verification.GetRemainingAttempts(email);
                            return Results.Json(
                                new { status = "wrong_code", message = "Incorrect verification code. " + attemptsLeft + " attempt(s) remaining.", attemptsRemaining = attemptsLeft },
                                statusCode: 400);

                        case VerificationResult.Expired:
                            return Results.Json(
                                new { status = "expired", message = "Verification code has expired. Please request a new one." },
                                statusCode: 400);

                        case VerificationResult.TemporarilyBlacklisted:
                            return Results.Json(
                                new { status = "blocked", message = "Too many failed attempts. Please try again in 5 minutes." },
                                statusCode: 403);

                        case VerificationResult.PermanentlyBlacklisted:
                            return Results.Json(
                                new { status = "blocked", message = "This email address has been permanently blocked." },
                                statusCode: 403);

                        case VerificationResult.NoPending:
                            return Results.Json(
                                new { status = "expired", message = "No pending verification found. Please request a new code." },
                                statusCode: 400);
                    }
                }

                // Not verified, no code provided — send verification email
                try
                {
                    var code = verification.CreateVerification(email);
                    await emailSender.SendVerificationAsync(email, code);

                    return Results.Ok(new { status = "verification_required", message = "A verification code has been sent to your email address." });
                }
                catch (Exception ex)
                {
                    return Results.Json(
                        new { status = "error", message = "Failed to send verification email.", detail = ex.Message },
                        statusCode: 500);
                }
            })
            .WithName("Contact");

            app.Run();
        }
    }
}
