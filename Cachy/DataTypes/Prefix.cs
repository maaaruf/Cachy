namespace Cachy.DataTypes;

public struct Prefix
{
    private readonly string _value;

    public Prefix(string value)
    {
        _value = value ?? string.Empty;
    }

    public Prefix()
    {
        _value = string.Empty;
    }

    public override string ToString()
    {
        return _value;
    }
}

