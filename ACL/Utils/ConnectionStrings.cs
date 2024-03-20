namespace ACL.Utils;

public class ConnectionStrings
{
    public string Legacy { get; }
    public string Bubble { get; }

    public ConnectionStrings(string legacy, string bubble)
    {
        Legacy = legacy;
        Bubble = bubble;
    }
}
