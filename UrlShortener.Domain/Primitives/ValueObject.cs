namespace UrlShortener.Domain.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public bool Equals(ValueObject? other)
    {
        return other is not null && ValuesAreEqual(other);
    }

    public abstract IEnumerable<object> GetAtomicValue();

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && ValuesAreEqual(other);
    }

    public override int GetHashCode()
    {
        return GetAtomicValue().Aggregate(0, HashCode.Combine);
    }

    private bool ValuesAreEqual(ValueObject other)
    {
        return GetAtomicValue().SequenceEqual(other.GetAtomicValue());
    }
}