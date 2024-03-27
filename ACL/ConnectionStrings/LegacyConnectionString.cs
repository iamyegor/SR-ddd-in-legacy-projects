namespace ACL.ConnectionStrings;

public static class LegacyConnectionString
{
    public static string Value { get; private set; } = null!;

    public static void Set(string connectionString)
    {
        Value = connectionString;
    }
}
