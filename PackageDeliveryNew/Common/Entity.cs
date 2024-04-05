namespace PackageDeliveryNew.Common;

public abstract class Entity<TId>
{
    public TId Id { get; }

    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        Entity<TId>? other = obj as Entity<TId>;

        if (ReferenceEquals(other, null))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id!.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id!.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
        {
            return true;
        }

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b)
    {
        return !(a == b);
    }
}
