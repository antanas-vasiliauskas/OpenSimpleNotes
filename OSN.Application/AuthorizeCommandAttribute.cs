namespace OSN.Application;

[AttributeUsage(AttributeTargets.Class)]
public class AuthorizeCommandAttribute : Attribute
{
    public string Policy { get; set; } = string.Empty;

    public AuthorizeCommandAttribute() { }

    public AuthorizeCommandAttribute(string policy)
    {
        Policy = policy;
    }
}
