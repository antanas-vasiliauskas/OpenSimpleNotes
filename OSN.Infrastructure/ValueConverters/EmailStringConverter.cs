using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OSN.Domain.ValueObjects;

namespace OSN.Infrastructure.ValueConverters;

public class EmailStringConverter : ValueConverter<EmailString, string>
{
    public EmailStringConverter() 
        : base(
            emailString => emailString.NormalizedValue,
            stringValue => EmailString.Create(stringValue)
          )
    {
    }
}