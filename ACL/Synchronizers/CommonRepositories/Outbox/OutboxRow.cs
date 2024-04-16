namespace ACL.Synchronizers.CommonRepositories.Outbox;

public class OutboxRow
{
    public int Id { get; set; }
    public string Content { get; set; } = null!; 
}