using OSN.Domain.ValueObjects;

namespace OSN.Test.UnitTests.Utils;

/// <summary>
/// Unit tests for the EmailString value object.
/// Tests cover email validation, normalization for different providers, and edge cases.
/// </summary>
public class EmailStringTests
{
    #region Gmail/Google Email Normalization Tests

    [Fact]
    public void EmailString_Create_GmailWithDots_RemovesDots()
    {
        // Arrange
        var email = "john.doe@gmail.com";
        var expected = "johndoe@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GmailWithMultipleDots_RemovesAllDots()
    {
        // Arrange
        var email = "j.o.h.n.d.o.e@gmail.com";
        var expected = "johndoe@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GmailWithPlusSign_RemovesPlusAndEverythingAfter()
    {
        // Arrange
        var email = "johndoe+newsletter@gmail.com";
        var expected = "johndoe@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GmailWithDotsAndPlusSign_RemovesDotsAndPlusContent()
    {
        // Arrange
        var email = "john.doe+shopping@gmail.com";
        var expected = "johndoe@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GmailWithUppercase_ConvertToLowercase()
    {
        // Arrange
        var email = "JOHN.DOE@GMAIL.COM";
        var expected = "johndoe@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GoogleMailDomain_AppliesGoogleRules()
    {
        // Arrange
        var email = "john.doe+test@googlemail.com";
        var expected = "johndoe@googlemail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GmailWithComplexPlusContent_RemovesEverythingAfterFirstPlus()
    {
        // Arrange
        var email = "user+tag1+tag2@gmail.com";
        var expected = "user@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    #endregion

    #region Non-Google Email Normalization Tests

    [Fact]
    public void EmailString_Create_RegularEmail_ConvertsToLowercaseAndRemovesPlus()
    {
        // Arrange
        var email = "JOHN.DOE+newsletter@OUTLOOK.COM";
        var expected = "john.doe@outlook.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_YahooWithDots_KeepsDotsButRemovesPlus()
    {
        // Arrange
        var email = "john.doe+spam@yahoo.com";
        var expected = "john.doe@yahoo.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_OutlookWithPlusSign_RemovesPlusSign()
    {
        // Arrange
        var email = "user+tag@outlook.com";
        var expected = "user@outlook.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_CorporateEmail_ConvertsToLowercaseAndRemovesPlus()
    {
        // Arrange
        var email = "John.Doe+work@COMPANY.COM";
        var expected = "john.doe@company.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_NonGmailWithDots_PreservesDots()
    {
        // Arrange
        var email = "user.name@outlook.com";
        var expected = "user.name@outlook.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public void EmailString_Create_NullEmail_ThrowsArgumentException()
    {
        // Arrange
        string? email = null;

        // Act & Assert
        var action = () => EmailString.Create(email!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var email = "";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_WhitespaceEmail_ThrowsArgumentException()
    {
        // Arrange
        var email = "   ";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_InvalidFormat_ThrowsArgumentException()
    {
        // Arrange
        var email = "notanemail";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_NoAtSymbol_ThrowsArgumentException()
    {
        // Arrange
        var email = "johndoegmail.com";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_MultipleAtSymbols_ThrowsArgumentException()
    {
        // Arrange
        var email = "john@doe@gmail.com";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_EmptyLocalPart_ThrowsArgumentException()
    {
        // Arrange
        var email = "@gmail.com";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_EmptyDomainPart_ThrowsArgumentException()
    {
        // Arrange
        var email = "johndoe@";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_GmailWithOnlyPlusSign_RemovesPlusSign()
    {
        // Arrange
        var email = "user+@gmail.com";
        var expected = "user@gmail.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_NonGmailWithOnlyPlusSign_RemovesPlusSign()
    {
        // Arrange
        var email = "user+@outlook.com";
        var expected = "user@outlook.com";

        // Act
        var result = EmailString.Create(email);

        // Assert
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public void EmailString_Create_GmailWithOnlyDots_ThrowsArgumentException()
    {
        // Arrange
        var email = "....@gmail.com";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    [Fact]
    public void EmailString_Create_EmailStartingWithPlus_ThrowsArgumentException()
    {
        // Arrange
        var email = "+tag@gmail.com";

        // Act & Assert
        var action = () => EmailString.Create(email);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format. (Parameter 'email')");
    }

    #endregion

    #region Real-World Scenarios

    [Theory]
    [InlineData("john.doe@gmail.com", "jane.doe@gmail.com", false)]
    [InlineData("john.doe@gmail.com", "johndoe@gmail.com", true)]
    [InlineData("john.doe+newsletter@gmail.com", "johndoe@gmail.com", true)]
    [InlineData("JOHN.DOE@GMAIL.COM", "johndoe@gmail.com", true)]
    [InlineData("john.doe+tag@yahoo.com", "john.doe@yahoo.com", true)]
    [InlineData("user+tag1@gmail.com", "user+tag2@gmail.com", true)]
    [InlineData("user.name+work@outlook.com", "user.name@outlook.com", true)]
    public void EmailString_Create_RealWorldScenarios_HandlesEmailEquivalency(string email1, string email2, bool shouldBeEqual)
    {
        // Act
        var emailString1 = EmailString.Create(email1);
        var emailString2 = EmailString.Create(email2);

        // Assert
        if (shouldBeEqual)
        {
            emailString1.NormalizedValue.Should().Be(emailString2.NormalizedValue, 
                $"because '{email1}' and '{email2}' should normalize to the same email");
        }
        else
        {
            emailString1.NormalizedValue.Should().NotBe(emailString2.NormalizedValue, 
                $"because '{email1}' and '{email2}' should not normalize to the same email");
        }
    }

    [Theory]
    [InlineData("user+work@gmail.com", "user@gmail.com")]
    [InlineData("user+work@yahoo.com", "user@yahoo.com")]
    [InlineData("user+work@outlook.com", "user@outlook.com")]
    [InlineData("john.doe+newsletter@gmail.com", "johndoe@gmail.com")]
    [InlineData("john.doe+newsletter@yahoo.com", "john.doe@yahoo.com")]
    public void EmailString_Create_UniversalPlusRemoval_WorksForAllProviders(string inputEmail, string expectedEmail)
    {
        // Act
        var result = EmailString.Create(inputEmail);

        // Assert
        result.NormalizedValue.Should().Be(expectedEmail, 
            $"Plus sign removal should work for all email providers");
    }

    #endregion
}