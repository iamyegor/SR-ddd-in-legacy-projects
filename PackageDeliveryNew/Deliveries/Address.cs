using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }

    public Address(string street, string city, string state, string zipCode)
    {
        Street = Guard.Against.NullOrWhiteSpace(street);
        City = Guard.Against.NullOrWhiteSpace(city);
        State = Guard.Against.NullOrWhiteSpace(state);
        ZipCode = Guard.Against.NullOrWhiteSpace(zipCode);
    }

    private Address() { }

    protected override IEnumerable<object?> GetPropertiesForComparison()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
    }
}
