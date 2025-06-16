using System.Text.Json.Serialization;

namespace OSN.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Employee,
    BusinessOwner,
    SuperAdmin
}