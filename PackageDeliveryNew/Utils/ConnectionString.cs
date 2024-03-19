namespace PackageDeliveryNew.Utils;

public static class ConnectionString
{
    public static string Value { get; private set; }

    public static void Set(string connectionString)
    {
        Value = connectionString;
    }
}
