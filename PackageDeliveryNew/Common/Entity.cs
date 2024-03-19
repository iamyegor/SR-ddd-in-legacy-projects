﻿using Ardalis.GuardClauses;

namespace PackageDeliveryNew.Common;

public abstract class Entity
{
    public int Id { get; }

    protected Entity(int id)
    {
        Guard.Against.NegativeOrZero(id);
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        Entity? other = obj as Entity;

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

        if (Id == 0 || other.Id == 0)
        {
            return false;
        }

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(Entity? a, Entity? b)
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

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }
}
