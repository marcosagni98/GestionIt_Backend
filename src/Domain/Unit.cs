namespace Domain;

/// <summary>
/// Represents a value that signifies the absence of a meaningful value,
/// similar to a "void" return type but usable in contexts where a 
/// value is required.
/// </summary>
public class Unit
{
    public static readonly Unit Value = new Unit();
    private Unit() { }
}
