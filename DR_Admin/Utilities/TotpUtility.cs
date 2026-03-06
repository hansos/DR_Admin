using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ISPAdmin.Utilities;

public static class TotpUtility
{
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string GenerateSharedKey(int byteLength = 20)
    {
        var bytes = new byte[byteLength];
        RandomNumberGenerator.Fill(bytes);
        return Base32Encode(bytes);
    }

    public static string BuildOtpAuthUri(string issuer, string accountName, string sharedKey)
    {
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedAccount = Uri.EscapeDataString(accountName);
        return $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={sharedKey}&issuer={encodedIssuer}&digits=6&period=30&algorithm=SHA1";
    }

    public static bool ValidateCode(string sharedKey, string code, int allowedTimeDriftWindows = 1)
    {
        if (string.IsNullOrWhiteSpace(sharedKey) || string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var cleanedCode = new string(code.Where(char.IsDigit).ToArray());
        if (cleanedCode.Length != 6)
        {
            return false;
        }

        byte[] key;
        try
        {
            key = Base32Decode(sharedKey);
        }
        catch
        {
            return false;
        }

        var unixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var currentCounter = unixSeconds / 30;

        for (var i = -allowedTimeDriftWindows; i <= allowedTimeDriftWindows; i++)
        {
            var generatedCode = GenerateCode(key, currentCounter + i);
            if (generatedCode == cleanedCode)
            {
                return true;
            }
        }

        return false;
    }

    private static string GenerateCode(byte[] key, long counter)
    {
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(counterBytes);

        var offset = hash[^1] & 0x0F;
        var binaryCode = ((hash[offset] & 0x7F) << 24)
                         | (hash[offset + 1] << 16)
                         | (hash[offset + 2] << 8)
                         | hash[offset + 3];

        var otp = binaryCode % 1_000_000;
        return otp.ToString("D6", CultureInfo.InvariantCulture);
    }

    private static string Base32Encode(byte[] data)
    {
        if (data.Length == 0)
        {
            return string.Empty;
        }

        var output = new StringBuilder((int)Math.Ceiling(data.Length / 5d) * 8);
        int buffer = data[0];
        int next = 1;
        int bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xFF;
                    bitsLeft += 8;
                }
                else
                {
                    int pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            var index = 0x1F & (buffer >> (bitsLeft - 5));
            bitsLeft -= 5;
            output.Append(Base32Alphabet[index]);
        }

        return output.ToString();
    }

    private static byte[] Base32Decode(string input)
    {
        var cleanedInput = input.Trim().TrimEnd('=').Replace(" ", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
        if (cleanedInput.Length == 0)
        {
            return Array.Empty<byte>();
        }

        var byteCount = cleanedInput.Length * 5 / 8;
        var returnArray = new byte[byteCount];

        int curByte = 0;
        int bitsRemaining = 8;
        int mask = 0;
        int arrayIndex = 0;

        foreach (var c in cleanedInput)
        {
            var cValue = Base32Alphabet.IndexOf(c);
            if (cValue < 0)
            {
                throw new FormatException("Invalid base32 character.");
            }

            if (bitsRemaining > 5)
            {
                mask = cValue << (bitsRemaining - 5);
                curByte |= mask;
                bitsRemaining -= 5;
            }
            else
            {
                mask = cValue >> (5 - bitsRemaining);
                curByte |= mask;
                returnArray[arrayIndex++] = (byte)curByte;
                curByte = cValue << (3 + bitsRemaining);
                bitsRemaining += 3;
            }
        }

        if (arrayIndex != byteCount)
        {
            returnArray[arrayIndex] = (byte)curByte;
        }

        return returnArray;
    }
}
