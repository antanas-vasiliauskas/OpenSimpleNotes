using System.Text.RegularExpressions;

namespace OSN.Domain.ValueObjects;

/// <summary>
/// Represents a validated and normalized email address value object.
/// </summary>
public sealed record EmailString
{
    private static readonly Regex EmailValidationRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string NormalizedValue { get; }
    private EmailString(string normalizedValue)
    {
        NormalizedValue = normalizedValue;
    }

    public static EmailString Create(string email)
    {
        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        var normalizedEmail = NormalizeEmail(email);
        return new EmailString(normalizedEmail);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailValidationRegex.IsMatch(email);
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        var emailParts = email.Split('@');
        if (emailParts.Length != 2)
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        var localPart = emailParts[0].ToLowerInvariant();
        var domainPart = emailParts[1].ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domainPart))
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        // Remove everything after the first '+' sign (including the '+')
        var plusIndex = localPart.IndexOf('+');
        if (plusIndex >= 0)
        {
            localPart = localPart.Substring(0, plusIndex);
        }

        // Apply Google-specific normalization (remove dots) for gmail.com and googlemail.com
        if (string.Equals(domainPart, "gmail.com", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(domainPart, "googlemail.com", StringComparison.OrdinalIgnoreCase))
        {
            localPart = localPart.Replace(".", "");
        }

        // Validate that local part is not empty after normalization
        if (string.IsNullOrEmpty(localPart))
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        return $"{localPart}@{domainPart}";
    }
}