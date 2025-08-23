namespace OSN.Domain.Common.Global;

public sealed class Unit
{
    public static readonly Unit Value = new Unit();
    private Unit() { }
}