namespace Cachy.DataTypes;

public struct Suffix
{
    private readonly string _value;

    public Suffix(string value)
    {
        _value = value ?? string.Empty;
    }

    public Suffix()
    {
        _value = string.Empty;
    }

    public override string ToString()
    {
        return _value;
    }
}

