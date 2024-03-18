namespace PackageDeliveryNew.Common;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetPropertiesForComparison();

    public override bool Equals(object? obj)
    {
        ValueObject? valueObject = obj as ValueObject;

        if (ReferenceEquals(valueObject, null))
        {
            return false;
        }

        if (GetType() != valueObject.GetType())
        {
            return false;
        }

        return GetPropertiesForComparison().SequenceEqual(valueObject.GetPropertiesForComparison());
    }

    public override int GetHashCode()
    {
        return GetPropertiesForComparison()
            .Select(prop => prop?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b)
    {
        return !(a == b);
    }
}
