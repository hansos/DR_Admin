using System.Collections.Concurrent;

namespace DR_Home.Services;

public class ContactVerificationService
{
    private record PendingVerification(string Code, DateTime ExpiresAt);
    private record VerifiedEmail(DateTime VerifiedAt, int MessagesSent, DateTime? WindowStart);
    private record BlacklistEntry(DateTime? ExpiresAt); // null = permanent

    private readonly ConcurrentDictionary<string, PendingVerification> _pending = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, VerifiedEmail> _verified = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, BlacklistEntry> _blacklist = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _failedAttempts = new(StringComparer.OrdinalIgnoreCase);

    private const int MaxMessagesPerWindow = 3;
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan TempBlacklistDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan VerificationCodeExpiry = TimeSpan.FromMinutes(10);
    private const int TempBlacklistThreshold = 5;
    private const int PermBlacklistThreshold = 10;

    public bool IsBlacklisted(string email)
    {
        if (!_blacklist.TryGetValue(email, out var entry))
            return false;

        // Permanent
        if (entry.ExpiresAt == null)
            return true;

        // Still active
        if (entry.ExpiresAt > DateTime.UtcNow)
            return true;

        // Expired — remove
        _blacklist.TryRemove(email, out _);
        return false;
    }

    public bool IsVerified(string email) => _verified.ContainsKey(email);

    public bool CanSendMessage(string email)
    {
        if (!_verified.TryGetValue(email, out var entry))
            return false;

        if (entry.WindowStart == null || DateTime.UtcNow - entry.WindowStart.Value >= RateLimitWindow)
            return true;

        return entry.MessagesSent < MaxMessagesPerWindow;
    }

    public int? GetRateLimitRemainingSeconds(string email)
    {
        if (!_verified.TryGetValue(email, out var entry) || entry.WindowStart == null)
            return null;

        var elapsed = DateTime.UtcNow - entry.WindowStart.Value;
        if (elapsed >= RateLimitWindow)
            return null;

        return (int)Math.Ceiling((RateLimitWindow - elapsed).TotalSeconds);
    }

    public void RecordMessageSent(string email)
    {
        _verified.AddOrUpdate(
            email,
            _ => new VerifiedEmail(DateTime.UtcNow, 1, DateTime.UtcNow),
            (_, existing) =>
            {
                if (existing.WindowStart == null || DateTime.UtcNow - existing.WindowStart.Value >= RateLimitWindow)
                    return existing with { MessagesSent = 1, WindowStart = DateTime.UtcNow };

                return existing with { MessagesSent = existing.MessagesSent + 1 };
            });
    }

    public bool HasPendingVerification(string email) => _pending.ContainsKey(email);

    public string CreateVerification(string email)
    {
        var code = Random.Shared.Next(100, 1000).ToString();
        _pending[email] = new PendingVerification(code, DateTime.UtcNow.Add(VerificationCodeExpiry));
        return code;
    }

    public VerificationResult ValidateCode(string email, string code)
    {
        if (!_pending.TryGetValue(email, out var pending))
            return VerificationResult.NoPending;

        if (pending.ExpiresAt < DateTime.UtcNow)
        {
            _pending.TryRemove(email, out _);
            return VerificationResult.Expired;
        }

        if (string.Equals(pending.Code, code.Trim(), StringComparison.Ordinal))
        {
            _pending.TryRemove(email, out _);
            _failedAttempts.TryRemove(email, out _);
            _verified[email] = new VerifiedEmail(DateTime.UtcNow, 0, null);
            return VerificationResult.Success;
        }

        // Wrong code — increment total failed attempts
        var totalAttempts = _failedAttempts.AddOrUpdate(email, 1, (_, count) => count + 1);

        if (totalAttempts >= PermBlacklistThreshold)
        {
            _pending.TryRemove(email, out _);
            _blacklist[email] = new BlacklistEntry(ExpiresAt: null);
            return VerificationResult.PermanentlyBlacklisted;
        }

        if (totalAttempts >= TempBlacklistThreshold)
        {
            _pending.TryRemove(email, out _);
            _blacklist[email] = new BlacklistEntry(DateTime.UtcNow.Add(TempBlacklistDuration));
            return VerificationResult.TemporarilyBlacklisted;
        }

        return VerificationResult.WrongCode;
    }

    public int GetRemainingAttempts(string email)
    {
        _failedAttempts.TryGetValue(email, out var count);

        if (count < TempBlacklistThreshold)
            return TempBlacklistThreshold - count;

        return PermBlacklistThreshold - count;
    }
}

public enum VerificationResult
{
    Success,
    WrongCode,
    NoPending,
    Expired,
    TemporarilyBlacklisted,
    PermanentlyBlacklisted
}
