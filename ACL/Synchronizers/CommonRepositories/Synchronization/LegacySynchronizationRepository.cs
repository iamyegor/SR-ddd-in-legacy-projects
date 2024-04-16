using System.Data.SqlClient;

namespace ACL.Synchronizers.CommonRepositories.Synchronization;

public class LegacySynchronizationRepository
{
    public byte[] GetRowVersionFor(string name, SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public void SetSyncFlagsFalseFor(string name, byte[] rowVersion, SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}
