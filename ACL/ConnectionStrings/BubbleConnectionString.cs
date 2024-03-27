namespace ACL.ConnectionStrings;

public static class BubbleConnectionString
{
    public static string Value { get; private set; } = null!;

    public static void Set(string connectionString)
    {
        Value = connectionString;
    }
}
