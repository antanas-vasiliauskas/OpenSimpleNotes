# Email Normalization

## Overview

The Open Simple Notes application implements email normalization through the `EmailString` value object to ensure consistent handling of user email addresses across all authentication methods. This prevents duplicate accounts and security issues.

## Implementation

### EmailString Value Object

The `EmailString` class in `OSN.Domain/ValueObjects/EmailString.cs` provides:

- **Create(string email)**: Creates validated and normalized email instances
- **NormalizedValue**: Gets the normalized email string
- Strict validation with exceptions for invalid formats

### Normalization Rules

#### Universal Rules (All Providers)

1. **Lowercase conversion**: `JOHN@GMAIL.COM` → `john@gmail.com`
2. **Plus sign removal**: `user+tag@gmail.com` → `user@gmail.com`

#### Google-Specific Rules

For `@gmail.com` and `@googlemail.com`:

3. **Dot removal**: `john.doe@gmail.com` → `johndoe@gmail.com`

#### Other Providers

- Apply universal rules only
- Preserve dots: `john.doe@outlook.com` stays unchanged

## Usage

### Authentication Handlers

Handlers use `EmailString.Create()` to validate and normalize email from request:

```csharp
    var emailString = EmailString.Create(request.Email);
    // Use emailString for database operations
```

### Database Storage

- `User.Email` is `EmailString?` (nullable for anonymous users)
- `PendingVerification.Email` is `EmailString` (required)
- Entity Framework converts via `EmailStringConverter`


## Database Constraints

PostgreSQL check constraints enforce normalization:

- Basic email format validation
- No uppercase letters
- No plus signs in local part
- No dots in Gmail local parts

## Examples

| Input | Normalized | Provider |
|-------|------------|----------|
| `John.Doe+test@Gmail.Com` | `johndoe@gmail.com` | Gmail |
| `user+tag@yahoo.com` | `user@yahoo.com` | Yahoo |
| `User.Name@Outlook.COM` | `user.name@outlook.com` | Outlook |

## Testing

Tests in `OSN.Test.UnitTests/Utils/EmailStringTests.cs` cover normalization rules, validation, and edge cases.